using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSMessagesController : BaseApiController
    {
        [Route("messages/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Message.Count(Keywords: Query);
            var list = Entities.Message.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Message.DefaultSort,
                List = list.Select(e => e.ListJson()),
                Title = "Messages"
            };

            return Success(data);
        }

        [Route("messages/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var message = Entities.Message.Find(Id: Id);
            if (message.Id == 0)
                return Error("Message not found");

            var thread = Entities.Message.List(ParentId: Id, Sort: "Id");

            if (message.StatusId == Entities.MessageStatus.Unread)
            {
                message.StatusId = Entities.MessageStatus.Read;
                message.Save();
            }

            var reply = new Entities.Message
            {
                ParentId = message.Id,
                StatusId = Entities.MessageStatus.Unread,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                User = CurrentUser,
                Title = $"REPLY: {message.Title}"
            };

            var data = new
            {
                Entity = message.DetailJson(),
                Thread = thread.Select(e => e.DetailJson()),
                Reply = reply.DetailJson()
            };

            return Success(data);
        }

        [Route("messages/reply")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Message Message)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            Message.StatusId = Entities.MessageStatus.Unread;

            if (Message.Save(SendNotification: true))
                return Find(Message.ParentId.Value);

            return Error();
        }
    }
}
