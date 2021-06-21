using Agrishare.Core.Entities;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class NotificationsController : BaseApiController
    {
        [Route("notifications/offering")]
        [AcceptVerbs("GET")]
        public object OfferingList(int PageIndex = 0, int PageSize = 25)
        {
            var list = Notification.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id, GroupId: NotificationGroup.Offering);
            return Success(new
            {
                List = list.Select(e => e.AppDashboardJson())
            });
        }

        [Route("notifications/seeking")]
        [AcceptVerbs("GET")]
        public object SeekingList(int PageIndex = 0, int PageSize = 25)
        {
            var list = Notification.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id, GroupId: NotificationGroup.Seeking);
            return Success(new
            {
                List = list.Select(e => e.AppDashboardJson())
            });
        }

        [Route("notifications/read")]
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
