using Agrishare.API;
using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsBookingsController : BaseApiController
    {
        [Route("bookings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, [FromUri] BookingFilterModel Filter)
        {
            var recordCount = Entities.Booking.Count(UserId: Filter.UserId, AgentId: Filter.AgentId, Status: Filter.Status, StartDate: Filter.StartDate, EndDate: Filter.EndDate, CategoryId: Filter.Category);
            var list = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: Filter.UserId, AgentId: Filter.AgentId, Status: Filter.Status, StartDate: Filter.StartDate, EndDate: Filter.EndDate, CategoryId: Filter.Category);
            var title = "Bookings";

            if (Filter.UserId > 0)
            {
                var user = Entities.User.Find(Id: Filter.UserId);
                if (user != null)
                    title = $"{user.FirstName} {user.LastName}";
            }

            if (Filter.AgentId > 0)
            {
                var agent = Entities.Agent.Find(Id: Filter.AgentId);
                if (agent != null)
                    title = agent.Title;
            }

            if (Filter.Status != Entities.BookingStatus.None)
                title = $"{Filter.Status}".ExplodeCamelCase();

            if (Filter.Category > 0)
            {
                var category = Entities.Category.Find(Id: Filter.Category);
                if (category != null)
                    title = category.Title;
            }

            var summary = Entities.Booking.Summary();

            var Statuses = EnumInfo.ToList<Entities.BookingStatus>().Where(s => s.Id != (int)Entities.BookingStatus.None).ToList();
            Statuses.Insert(0, new EnumDescriptor { Id = -1, Title = "All" });

            var Categories = Entities.Category.List(ParentId: 0);
            Categories.Insert(0, new Entities.Category { Id = 0, Title = "All" });

            var graphData = Entities.Booking.Graph(UserId: Filter.UserId, AgentId: Filter.AgentId, Status: Filter.Status, StartDate: Filter.StartDate ?? DateTime.Now.AddMonths(-6), EndDate: Filter.EndDate ?? DateTime.Now, CategoryId: Filter.Category);
            var Graph = new List<object>();
            foreach(var item in graphData)
            {
                Graph.Add(new
                {
                    Label = new DateTime(item.Year, item.Month, 1).ToString("MMM yy"),
                    item.Count,
                    Height = (decimal)item.Count / (decimal)graphData.Max(d => d.Count) * 100M
                });
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title,
                Summary = new
                {
                    Complete = new
                    {
                        Me = summary.Where(p => p.Status == Entities.BookingStatus.Complete && p.For == Entities.BookingFor.Me).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Friend = summary.Where(p => p.Status == Entities.BookingStatus.Complete && p.For == Entities.BookingFor.Friend).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Group = summary.Where(p => p.Status == Entities.BookingStatus.Complete && p.For == Entities.BookingFor.Group).Select(p => p.Count).DefaultIfEmpty(0).Sum()
                    },

                    InProgress = new
                    {
                        Me = summary.Where(p => p.Status == Entities.BookingStatus.InProgress && p.For == Entities.BookingFor.Me).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Friend = summary.Where(p => p.Status == Entities.BookingStatus.InProgress && p.For == Entities.BookingFor.Friend).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Group = summary.Where(p => p.Status == Entities.BookingStatus.InProgress && p.For == Entities.BookingFor.Group).Select(p => p.Count).DefaultIfEmpty(0).Sum()
                    },

                    Approved = new
                    {
                        Me = summary.Where(p => p.Status == Entities.BookingStatus.Approved && p.For == Entities.BookingFor.Me).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Friend = summary.Where(p => p.Status == Entities.BookingStatus.Approved && p.For == Entities.BookingFor.Friend).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                        Group = summary.Where(p => p.Status == Entities.BookingStatus.Approved && p.For == Entities.BookingFor.Group).Select(p => p.Count).DefaultIfEmpty(0).Sum()
                    },

                    Pending = summary.Where(p => p.Status == Entities.BookingStatus.Pending).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                    Declined = summary.Where(p => p.Status == Entities.BookingStatus.Declined).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                    Cancelled = summary.Where(p => p.Status == Entities.BookingStatus.Cancelled).Select(p => p.Count).DefaultIfEmpty(0).Sum(),
                    Incomplete = summary.Where(p => p.Status == Entities.BookingStatus.Incomplete).Select(p => p.Count).DefaultIfEmpty(0).Sum(),

                    Graph
                },
                Statuses,
                Categories = Categories.Select(c => c.Json())
            };

            return Success(data);
        }

        [Route("bookings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Booking.Find(Id: Id).Json(),
                Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
            };

            return Success(data);
        }

        [Route("bookings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Booking Booking)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Booking.Save())
                return Success(new
                {
                    Entity = Booking.Json()
                });

            return Error();
        }

        [Route("bookings/transactions/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);
            foreach (var transaction in transactions)
                transaction.RequestEcoCashStatus();

            var data = new
            {
                Entity = booking.Json(),
                Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                Feedback = $"Finished updating transaction statuses"
            };

            return Success(data);
        }

        [Route("bookings/cancel")]
        [AcceptVerbs("GET")]
        public object CancelBooking(int Id = 0)
        {
            var booking = Entities.Booking.Find(Id: Id);
            if (booking == null || booking.Id == 0)
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("You can not cancel a completed booking");

            if (booking.StatusId == Entities.BookingStatus.InProgress)
                return Error("You can not cancel a paid booking that is not complete");

            booking.StatusId = Entities.BookingStatus.Cancelled;
            if (booking.Save())
            {
                var transactions = Entities.Transaction.List(BookingId: booking.Id);
                foreach (var transaction in transactions)
                    transaction.RequestEcoCashRefund();

                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.BookingCancelled);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: false);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.CancelBooking, ServiceId: booking.Service.Id, CategoryId: booking.Service.CategoryId, BookingId: booking.Id);

                var data = new
                {
                    Entity = booking.Json(),
                    Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                    Feedback = $"Booking was cancelled"
                };

                return Success(data);
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/transactions/refund")]
        [AcceptVerbs("GET")]
        public object RefundTransaction(int Id = 0)
        {
            var transaction = Entities.Transaction.Find(Id: Id);

            if (transaction.RequestEcoCashRefund())
            {
                var data = new
                {
                    Entity = Entities.Booking.Find(Id: Id).Json(),
                    Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
                };

                return Success(data);
            }
            else
                return Error("Unable to process refund");
        }

        [Route("bookings/transactions/find")]
        [AcceptVerbs("GET")]
        public object FindTransaction(int Id = 0)
        {
            var transaction = Entities.Transaction.Find(Id: Id);

            var data = new
            {
                Entity = transaction.Json(),
                Log = transaction.Log.Trim(),
                transaction.ClientCorrelator
            };

            return Success(data);
        }
    }
}
