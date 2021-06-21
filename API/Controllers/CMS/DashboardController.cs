using Agrishare.API;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
            if (startDate.Year < 2018)
                startDate = new DateTime(2018, 1, 1);
            var endDate = (EndDate ?? DateTime.MaxValue).EndOfDay();
            if (endDate > DateTime.Now)
                endDate = DateTime.Now;

            var timeSpan = endDate - startDate;
            var uniqueUser = Type.Equals("User");

            var totalBookingAmount = Entities.Booking.TotalAmountPaid(startDate, endDate, RegionId: CurrentRegion.Id);
            var agrishareCommission = Entities.Booking.TotalAgriShareCommission(startDate, endDate, RegionId: CurrentRegion.Id);
            var agentsCommission = Entities.Booking.TotalAgentsCommission(startDate, endDate, RegionId: CurrentRegion.Id);
            var feesIncurred = Entities.Booking.TotalFeesIncurred(startDate, endDate, RegionId: CurrentRegion.Id);

            var locations = Entities.Booking.List(StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id);

            var graphData = Entities.Booking.Graph(StartDate: DateTime.Now.AddMonths(-12), EndDate: DateTime.Now, Count: 12, RegionId: CurrentRegion.Id);
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

            var profitData = Entities.Journal.Graph(StartDate: startDate, EndDate: endDate, View: profitView, RegionId: CurrentRegion.Id);

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

            var ageGenderData = Entities.User.GetAgeGenderData(RegionId: CurrentRegion.Id);
            decimal max = ageGenderData.Count > 0 ? ageGenderData.Max(e => e.Count) : 1;
            decimal total = ageGenderData.Count > 0 ? ageGenderData.Sum(e => e.Count) : 1;
            var ageGenderGraph = new List<object>();
            for (var i = 0; i < Entities.User.AgeGenderData.AgeRanges.Count; i++)
                ageGenderGraph.Add(new
                {
                    range = Entities.User.AgeGenderData.AgeRanges[i],
                    male = ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Male)?.Count ?? 0,
                    malePercent = (ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Male)?.Count ?? 0) / total,
                    maleHeight = (ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Male)?.Count ?? 0) / max * 100M,
                    female = ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Female)?.Count ?? 0,
                    femalePercent = (ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Female)?.Count ?? 0) / total,
                    femaleHeight = (ageGenderData.FirstOrDefault(e => e.AgeRangeIndex == i && e.Gender == Entities.Gender.Female)?.Count ?? 0) / max * 100M
                });

            var smsBalance = SMS.GetBalance(CurrentRegion);

            var data = new
            {
                activeListingCount = Entities.Listing.Count(Status: Entities.ListingStatus.Live, RegionId: CurrentRegion.Id),
                activeUsers = Entities.Counter.ActiveUsers(startDate, endDate, RegionId: CurrentRegion.Id), //Entities.Counter.Count(UniqueUser: true),
                completeBookingCount = Entities.Booking.Count(Status: Entities.BookingStatus.Complete, RegionId: CurrentRegion.Id),
                totalBookingAmount,
                totalRegistrations = Entities.Counter.Count(Event: Entities.Counters.Register, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                agrishareCommission,
                agentsCommission,
                feesIncurred,
                searchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                matchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                bookingCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                confirmCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                paidCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                completeCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id)
                },
                locations = locations.Select(o => new { o.Latitude, o.Longitude }),
                smsBalance,
                bookingsGraph,
                profitGraph,
                ageGenderGraph,
                currentUsdRate = Entities.Journal.CurrentRate
            };

            return Success(data);
        }
    }
}
