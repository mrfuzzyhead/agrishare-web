using Agrishare.API;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator, Dashboard")]
    [RoutePrefix("cms")]
    public class CmsDashboardController : BaseApiController
    {
        [Route("dashboard/summary")]
        [AcceptVerbs("GET")]
        public object Summary(string Type = "", DateTime? StartDate = null, DateTime? EndDate = null, int Category = 0)
        {
            var startDate = (StartDate ?? DateTime.MinValue).StartOfDay();
            if (startDate.Year < 2019)
                startDate = new DateTime(2019, 1, 1);
            var endDate = (EndDate ?? DateTime.MaxValue).EndOfDay();
            if (endDate > DateTime.Now)
                endDate = DateTime.Now;

            var timeSpan = endDate - startDate;
            var uniqueUser = Type.Equals("User");

            var totalBookingAmount = Entities.Booking.TotalAmountPaid(startDate, endDate);
            var agrishareCommission = Entities.Booking.TotalAgriShareCommission(startDate, endDate);
            var agentsCommission = Entities.Booking.TotalAgentsCommission(startDate, endDate);
            var feesIncurred = Entities.Booking.TotalFeesIncurred(startDate, endDate);

            var locations = Entities.Booking.List(StartDate: startDate, EndDate: endDate);

            var graphData = Entities.Booking.Graph(StartDate: DateTime.Now.AddMonths(-12), EndDate: DateTime.Now, Count: 12);
            var bookingsGraph = new List<object>();
            foreach (var item in graphData)
            {
                bookingsGraph.Add(new
                {
                    Label = new DateTime(item.Year, item.Month, 1).ToString("MMM yy"),
                    item.Count,
                    Height = (decimal)item.Count / (decimal)graphData.Max(d => d.Count) * 100M
                });
            }

            var dateLabels = new List<string>();
            var profitAmounts = new List<decimal>();

            var profitView = Entities.Journal.GraphView.Month;
            if (timeSpan.TotalDays <= 7)
                profitView = Entities.Journal.GraphView.Day;
            else if (timeSpan.TotalDays <= 90)
                profitView = Entities.Journal.GraphView.Week;

            var profitData = Entities.Journal.Graph(StartDate: startDate, EndDate: endDate, View: profitView);

            var currentDate = startDate;
            while (currentDate < endDate)
            {
                var amount = 0M;

                if (profitView == Entities.Journal.GraphView.Day)
                {
                    dateLabels.Add(currentDate.ToString("d MMM"));
                    amount = profitData.FirstOrDefault(e => e.Day == currentDate.Day && e.Month == currentDate.Month && e.Year == currentDate.Year)?.Amount ?? 0;
                    currentDate = currentDate.AddDays(1);
                }
                else if (profitView == Entities.Journal.GraphView.Week)
                {
                    dateLabels.Add(currentDate.ToString("d MMM yy"));
                    var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    amount = profitData.FirstOrDefault(e => e.Week == week && e.Year == currentDate.Year)?.Amount ?? 0;
                    currentDate = currentDate.AddDays(7);
                }
                else if (profitView == Entities.Journal.GraphView.Month)
                {
                    dateLabels.Add(currentDate.ToString("MMM yy"));
                    amount = profitData.FirstOrDefault(e => e.Month == currentDate.Month && e.Year == currentDate.Year)?.Amount ?? 0;
                    currentDate = currentDate.AddMonths(1);
                }

                profitAmounts.Add(amount);
            }

            var profitGraph = new
            {
                Labels = dateLabels,
                Data = new List<object> { profitAmounts },
                Series = new List<string> { "Profit" }
            };

            var data = new
            {
                activeListingCount = Entities.Listing.Count(Status: Entities.ListingStatus.Live),
                activeUsers = Entities.Counter.ActiveUsers(startDate, endDate), //Entities.Counter.Count(UniqueUser: true),
                completeBookingCount = Entities.Booking.Count(Status: Entities.BookingStatus.Complete),
                totalBookingAmount,
                totalRegistrations = Entities.Counter.Count(Event: Entities.Counters.Register, StartDate: startDate, EndDate: endDate),
                agrishareCommission,
                agentsCommission,
                feesIncurred,
                searchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                matchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                bookingCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                confirmCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                paidCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                completeCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                locations = locations.Select(o => new { o.Latitude, o.Longitude }),
                smsBalance = SMS.GetBalance(),
                bookingsGraph,
                profitGraph
            };

            return Success(data);
        }
    }
}
