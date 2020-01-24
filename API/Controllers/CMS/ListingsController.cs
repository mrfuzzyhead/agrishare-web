﻿using Agrishare.API;
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
    public class CmsListingsController : BaseApiController
    {
        [Route("listings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, [FromUri] ListingFilterModel Filter)
        {
            var recordCount = Entities.Listing.Count(Keywords: Filter.Query, UserId: Filter.UserId, CategoryId: Filter.CategoryId);
            var list = Entities.Listing.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Filter.Query, UserId: Filter.UserId, CategoryId: Filter.CategoryId);
            var title = "Listings";

            if (Filter.UserId > 0)
            {
                var user = Entities.User.Find(Id: Filter.UserId);
                if (user != null)
                    title = $"{user.FirstName} {user.LastName}";
            }
            else if (Filter.CategoryId > 0)
            {
                var category = Entities.Category.Find(Id: Filter.CategoryId);
                if (category != null)
                    title = category.Title;
            }

            int total = 0, tractors = 0, lorries = 0, processors = 0, buses = 0, reviews = 0, onestar = 0;
            decimal averageRating = 0M;
            if (PageIndex == 0)
            {
                total = Entities.Listing.Count();
                tractors = Entities.Listing.Count(CategoryId: Entities.Category.TractorsId);
                lorries = Entities.Listing.Count(CategoryId: Entities.Category.LorriesId);
                processors = Entities.Listing.Count(CategoryId: Entities.Category.ProcessingId);
                buses = Entities.Listing.Count(CategoryId: Entities.Category.BusId);
                reviews = Entities.Rating.Count();
                onestar = Entities.Rating.Count(Stars: 1);
                averageRating = Entities.Rating.AverageRating();
            }

            var graphData = Entities.Listing.Graph(StartDate: DateTime.Now.AddMonths(-6), EndDate: DateTime.Now, CategoryId: Filter.CategoryId);
            var Graph = new List<object>();
            foreach (var item in graphData)
            {
                Graph.Add(new
                {
                    Label = new DateTime(item.Year, item.Month, 1).ToString("MMM yy"),
                    item.Count,
                    Height = item.Count / graphData.Max(d => d.Count) * 100
                });
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Listing.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title,
                Summary = new
                {
                    Total = total,
                    Tractors = tractors,
                    Lorries = lorries,
                    Processors = processors,
                    Buses = buses,
                    Reviews = reviews,
                    AverageRating = averageRating,
                    OneStar = onestar,
                    Graph
                }
            };

            return Success(data);
        }

        [Route("listings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Listing.Find(Id: Id).Json(IncludeUser: true),
                Categories = Entities.Category.List().Select(e => e.Json()),
                Conditions = EnumInfo.ToList<Entities.ListingCondition>(),
                Statuses = EnumInfo.ToList<Entities.ListingStatus>()
            };

            return Success(data);
        }

        [Route("listings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Listing Listing)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Listing.Save())
                return Success(new
                {
                    Entity = Listing.Json()
                });

            return Error();
        }

        [Route("listings/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var listing = Entities.Listing.Find(Id: Id);
            if (listing?.Id == 0)
                return Error("Listing not found");

            if (listing.Delete())
            {
                var bookings = Entities.Booking.List(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                foreach (var booking in bookings)
                {
                    booking.StatusId = Entities.BookingStatus.Cancelled;
                    if (booking.Save())
                    {
                        // TODO refund users

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
                        }.Save(Notify: true);

                        Entities.Counter.Hit(booking.UserId, Entities.Counters.CancelBooking, booking.Service.CategoryId, booking.Id);
                    }
                }

                return Success(new
                {
                    listing.Id
                });
            }

            return Error("An unknown error occurred");
        }

        /* Deleted */

        [Route("listings/deleted/list")]
        [AcceptVerbs("GET")]
        public object ListDeleted(int PageIndex, int PageSize, string Query = "", int UserId = 0)
        {
            var recordCount = Entities.Listing.Count(Keywords: Query, UserId: UserId, Deleted: true);
            var list = Entities.Listing.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, UserId: UserId, Deleted: true);
            var title = "Deleted Listings";

            if (UserId > 0)
            {
                var user = Entities.User.Find(Id: UserId);
                if (user != null)
                    title = $"{user.FirstName} {user.LastName}";
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Listing.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("listings/deleted/find")]
        [AcceptVerbs("GET")]
        public object FindDeleted(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Listing.Find(Id: Id, Deleted: true).Json(IncludeUser: true),
                Categories = Entities.Category.List().Select(e => e.Json()),
                Conditions = EnumInfo.ToList<Entities.ListingCondition>(),
                Statuses = EnumInfo.ToList<Entities.ListingStatus>()
            };

            return Success(data);
        }

        /* Map */

        [Route("listings/map")]
        [AcceptVerbs("GET")]
        public object MapList(double NELat, double NELng, double SWLat, double SWLng)
        {
            var list = Entities.Listing.MapList(NELat, NELng, SWLat, SWLng);
            return Success(list.Select(e => e.Json()));
        }
    }
}
