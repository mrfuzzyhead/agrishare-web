using Agrishare.API;
using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Entities;
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
            var status = Entities.BookingStatus.None;
            bool? paidOut = null;
            if (Filter.Status == 998)
                paidOut = false;
            else if (Filter.Status == 999)
                paidOut = true;
            else
                status = (Entities.BookingStatus)Enum.ToObject(typeof(Entities.BookingStatus), Filter.Status);

            var recordCount = Entities.Booking.Count(UserId: Filter.UserId, AgentId: Filter.AgentId, Status: status, StartDate: Filter.StartDate, EndDate: Filter.EndDate, CategoryId: Filter.Category, PaidOut: paidOut, RegionId: CurrentRegion.Id);
            var list = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: Filter.UserId, AgentId: Filter.AgentId, Status: status, StartDate: Filter.StartDate, EndDate: Filter.EndDate, CategoryId: Filter.Category, PaidOut: paidOut, Sort: "DateCreated DESC", RegionId: CurrentRegion.Id);
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

            if (status != Entities.BookingStatus.None)
                title = $"{Filter.Status}".ExplodeCamelCase();

            if (Filter.Category > 0)
            {
                var category = Entities.Category.Find(Id: Filter.Category);
                if (category != null)
                    title = category.Title;
            }

            var summary = Entities.Booking.Summary(RegionId: CurrentRegion.Id);

            var Statuses = EnumInfo.ToList<Entities.BookingStatus>().Where(s => s.Id != (int)Entities.BookingStatus.None).ToList();
            Statuses.Insert(0, new EnumDescriptor { Id = 0, Title = "All" });
            Statuses.Add(new EnumDescriptor { Id = 998, Title = "Supplier Payment Pending" });
            Statuses.Add(new EnumDescriptor { Id = 999, Title = "Supplier Paid" });

            var Categories = Entities.Category.List(ParentId: 0);
            Categories.Insert(0, new Entities.Category { Id = 0, Title = "All" });

            var graphData = Entities.Booking.Graph(UserId: Filter.UserId, AgentId: Filter.AgentId, Status: status, StartDate: Filter.StartDate ?? DateTime.Now.AddMonths(-6), EndDate: Filter.EndDate ?? DateTime.Now, CategoryId: Filter.Category, RegionId: CurrentRegion.Id);
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
            var booking = Entities.Booking.Find(Id: Id);
            var supplier = Entities.User.Find(booking.Listing.UserId);
            var transactions = Entities.Transaction.List(BookingId: Id);
            var tags = Entities.Tag.List();
            var comments = Entities.BookingComment.List(BookingId: booking.Id);

            var Currencies = new List<EnumDescriptor>();
            if (booking.Listing.RegionId == (int)Regions.Zimbabwe)
            {
                Currencies.Add(new EnumDescriptor { Id = (int)Currency.USD, Title = $"{Currency.USD}" });
                Currencies.Add(new EnumDescriptor { Id = (int)Currency.ZWL, Title = $"{Currency.ZWL}" });
            }
            else if (booking.Listing.RegionId == (int)Regions.Uganda)
            {
                Currencies.Add(new EnumDescriptor { Id = (int)Currency.USh, Title = $"{Currency.USh}" });
            }

            var data = new
            {
                Entity = booking.Json(),
                Supplier = supplier.Json(),
                Transactions = transactions.Select(e => e.Json()),
                Tags = tags.Select(e => e.Json()),
                Comments = comments.Select(e => e.Json()),
                Currencies,
                Payment = new
                {
                    Amount = booking.Price,
                    Currency = Currencies.First().Id
                }
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

        [Route("bookings/paidout")]
        [AcceptVerbs("GET")]
        public object PaidOut(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null)
                return Error("Booking not found");

            booking.PaidOut = true;
            if (booking.Save())
                return Find(BookingId);

            return Error();
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

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.CancelBooking, CategoryId: booking.Service.CategoryId, BookingId: booking.Id);

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

        /* Transactions */

        [Route("bookings/transactions/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null)
                return Error("Booking not found");

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
                Log = transaction.Log?.Trim(),
                transaction.ClientCorrelator
            };

            return Success(data);
        }

        /* Tags */

        [Route("bookings/tags/remove")]
        [AcceptVerbs("GET")]
        public object RemoveTag(int TagId, int BookingId)
        {
            if (Entities.BookingTag.Remove(TagId, BookingId))
                return Find(BookingId);
            return Error();
        }

        [Route("bookings/tags/add")]
        [AcceptVerbs("GET")]
        public object AddTag(int TagId, int BookingId)
        {
            if (Entities.BookingTag.Add(TagId, BookingId))
                return Find(BookingId);
            return Error();
        }

        /* Comments */

        [Route("bookings/comments/remove")]
        [AcceptVerbs("GET")]
        public object RemoveComment(int CommentId, int BookingId)
        {
            var comment = Entities.BookingComment.Find(CommentId);
            if (comment.Id == 0 || comment.UserId != CurrentUser.Id)
                return Error("Comment not found");

            if (comment.Delete())
                return Find(BookingId);

            return Error();
        }

        [Route("bookings/comments/add")]
        [AcceptVerbs("GET")]
        public object AddComment(string Text, int BookingId)
        {
            var comment = new Entities.BookingComment
            {
                User = CurrentUser,
                BookingId = BookingId,
                Text = Text
            };

            if (comment.Save())
                return Find(BookingId);

            return Error();
        }

        /* Payment */

        [Route("bookings/paid")]
        [AcceptVerbs("GET")]
        public object BookingPaid(int Id, Currency Currency, decimal Amount)
        {
            var booking = Booking.Find(Id: Id);
            if (booking == null || booking.Id == 0)
                return Error("Booking not found");

            if (booking.StatusId != BookingStatus.Approved && booking.StatusId != BookingStatus.Paid)
                return Error("Booking has already been updated");

            booking.StatusId = BookingStatus.InProgress;
            if (booking.Save())
            {
                decimal rate = 1;
                if (booking.Listing.Region.Id == (int)Regions.Zimbabwe && Currency == Currency.ZWL)
                    rate = Amount / booking.Price;

                new Journal
                {
                    Amount = booking.Price,
                    BookingId = booking.Id,
                    Date = DateTime.UtcNow,
                    EcoCashReference = string.Empty,
                    Reconciled = true,
                    Region = booking.Listing.Region,
                    Title = $"Payment received from {booking.User.Title} {booking.User.Telephone}",
                    TypeId = JournalType.Payment,
                    CurrencyAmount = Amount,
                    Currency = Currency,
                    UserId = booking.UserId
                }.Save();

                var transactionFee = TransactionFee.Find(booking.Price - booking.AgriShareCommission);
                if (transactionFee != null)
                {
                    if (transactionFee.FeeType == FeeType.Fixed)
                        booking.TransactionFee = transactionFee.Fee;
                    else
                        booking.TransactionFee = (booking.Price - booking.AgriShareCommission) * transactionFee.Fee;
                }

                booking.IMTT = (booking.Price - booking.AgriShareCommission) * Transaction.IMTT;
                booking.Save();

                var notifications = Notification.List(BookingId: booking.Id, Type: NotificationType.BookingConfirmed);
                foreach (var notification in notifications)
                {
                    notification.StatusId = NotificationStatus.Complete;
                    notification.Save();
                }

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Offering,
                    TypeId = NotificationType.PaymentReceived,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Seeking,
                    TypeId = NotificationType.PaymentReceived,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: false);

                Counter.Hit(UserId: booking.UserId, Event: Counters.CompletePayment, CategoryId: booking.Service.CategoryId, BookingId: booking.Id);

                var data = new
                {
                    Entity = booking.Json(),
                    Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                    Feedback = $"Booking updated"
                };

                return Success(data);
            }

            return Error("An unknown error occurred");
        }
    }
}
