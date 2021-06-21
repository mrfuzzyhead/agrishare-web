using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class AvialabilityController : BaseApiController
    {
        [Route("availability/check")]
        [AcceptVerbs("GET")]
        public object Availability(int ListingId, DateTime StartDate, DateTime EndDate, int Days = 1, int Volume = 0)
        {
            var listing = Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0)
                return Error("Listing not found");

            var bookings = Booking.List(ListingId: ListingId, StartDate: StartDate, EndDate: StartDate.AddDays(Days));

            var list = new List<object>();

            if (listing.CategoryId == Category.LandId)
            {
                if (listing.CategoryId == Category.LandId)
                    EndDate = StartDate.AddMonths(12);

                var date = StartDate;
                while (date <= EndDate)
                {
                    var start = date.StartOfDay();
                    var end = date.AddDays(Days - 1).EndOfDay();

                    var bookedVolume = bookings.Where(o =>
                        (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.InProgress || o.StatusId == BookingStatus.Pending || o.StatusId == BookingStatus.Incomplete) &&
                        o.StartDate <= end &&
                        o.EndDate >= start).Sum(e => e.TotalVolume);

                    var available = bookedVolume + Volume <= listing.Services.First().AvailableAcres;

                    list.Add(new
                    {
                        Date = date,
                        Available = available
                    });

                    date = date.AddMonths(1);
                }
            }
            else
            {
                var date = StartDate;
                while (date <= EndDate)
                {
                    var start = date.StartOfDay();
                    var end = date.AddDays(Days - 1).EndOfDay();

                    var available = bookings.Where(o =>
                        (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.InProgress || o.StatusId == BookingStatus.Pending || o.StatusId == BookingStatus.Incomplete) &&
                        o.StartDate <= end &&
                        o.EndDate >= start).Count() == 0;

                    list.Add(new
                    {
                        Date = date,
                        Available = available
                    });

                    date = date.AddDays(1);
                }
            }            

            return Success(new
            {
                Calendar = list
            });
        }
    }
}
