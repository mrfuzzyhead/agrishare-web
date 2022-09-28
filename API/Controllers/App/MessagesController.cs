using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class MessagesController : BaseApiController
    {
        [Route("messages")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 50)
        {
            var list = Entities.Message.List(UserId: CurrentUser.Id, PageIndex: PageIndex, PageSize: PageSize);
            return Success(new
            {
                List = list.Select(e => e.ListJson())
            });
        }

        [Route("message/thread")]
        [AcceptVerbs("GET")]
        public object Detail(int MessageId)
        {
            var message = Entities.Message.Find(Id: MessageId);

            if (message == null || message.User.Id != CurrentUser.Id)
                return Error("Message not found");

            if (message.StatusId == Entities.MessageStatus.Unread)
            {
                message.StatusId = Entities.MessageStatus.Read;
                message.Save();
            }

            var thread = Entities.Message.List(ParentId: message.Id).OrderBy(e => e.Id).ToList();
            thread.Insert(0, message);

            return Success(new
            {
                List = thread.Select(e => e.DetailJson())
            });
        }

        [Route("message/create")]
        [AcceptVerbs("POST")]
        public object Create(MessageModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var message = new Entities.Message
            {
                User = CurrentUser,
                Title = Model.Title,
                Content = Model.Content,
                StatusId = Entities.MessageStatus.Unread,
                RegionId = CurrentUser.RegionId
            };

            if (message.Save())
                return Success(new
                {
                    Message = message.DetailJson()
                });

            return Error();
        }

        [Route("message/reply")]
        [AcceptVerbs("POST")]
        public object Reply(MessageModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var original = Entities.Message.Find(Id: Model.ParentId);
            if (original == null || original.User.Id != CurrentUser.Id)
                return Error("Message not found");

            original.StatusId = Entities.MessageStatus.Unread;
            original.Save();

            var message = new Entities.Message
            {
                ParentId = Model.ParentId,
                User = CurrentUser,
                Title = $"REPLY: {original.Title}",
                Content = Model.Content,
                StatusId = Entities.MessageStatus.Unread,
                RegionId = CurrentUser.RegionId
            };

            if (message.Save())
                return Success(new
                {
                    Message = message.DetailJson()
                });

            return Error();
        }
    }
}
