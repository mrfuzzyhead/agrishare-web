using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class DashboardController : BaseApiController
    {
        [Route("dashboard/offering")]
        [AcceptVerbs("GET")]
        public object OfferingList()
        {
            var notifications = Notification.List(PageSize: 5, UserId: CurrentUser.Id, GroupId: NotificationGroup.Offering);
            var bookings = Booking.List(PageSize: 5, ListingUserId: CurrentUser.Id, ListingSupplierId: CurrentUser.Supplier?.Id ?? 0);

            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var monthlySpend = Booking.OfferingSummary(UserId: CurrentUser.Id, SupplierId: CurrentUser.Supplier?.Id ?? 0, StartDate: startDate);
            var totalSpend = Booking.OfferingSummary(UserId: CurrentUser.Id, SupplierId: CurrentUser.Supplier?.Id ?? 0);

            return Success(new
            {
                Notifications = notifications.Select(e => e.AppDashboardJson()),
                Bookings = bookings.Select(e => e.AppDashboardJson()),
                Summary = new
                {
                    Month = monthlySpend,
                    MonthCommission = monthlySpend * Entities.Transaction.AgriShareCommission,
                    Total = totalSpend,
                    TotalCommission = totalSpend * Entities.Transaction.AgriShareCommission
                }
            });
        }

        [Route("dashboard/seeking")]
        [AcceptVerbs("GET")]
        public object SeekingList()
        {
            var notifications = Notification.List(PageSize: 5, UserId: CurrentUser.Id, GroupId: NotificationGroup.Seeking);
            var bookings = Booking.List(PageSize: 5, UserId: CurrentUser.Id);

            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var summary = new
            {
                Month = Booking.SeekingSummary(CurrentUser.Id, startDate),
                Total = Booking.SeekingSummary(CurrentUser.Id),
                Commission = CurrentUser.Agent != null ? Booking.SeekingSummaryAgentCommission(CurrentUser.Id) : 0
            };

            var trending = Listing.List(PageIndex: 0, PageSize: 5, RegionId: CurrentRegion.Id, Trending: true);
           
            return Success(new
            {
                Notifications = notifications.Select(e => e.AppDashboardJson()),
                Bookings = bookings.Select(e => e.AppDashboardJson()),
                Trending = trending.Select(e => e.AppDashboardJson()),
                Summary = summary
            });
        }

        [Route("dashboard/notifications/read")]
        [AcceptVerbs("GET")]
        public object NotificationRead(int Id)
        {
            var notification = Notification.Find(Id);
            notification.StatusId = NotificationStatus.Complete;
            if (notification.Save())
                return Success();
            return Error();
        }
    }
}
