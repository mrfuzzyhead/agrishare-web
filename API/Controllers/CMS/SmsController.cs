using Agrishare.API;
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
    public class SmsController : BaseApiController
    {
        [Route("sms/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var entity = new SmsModel
            {
                RecipientCount = Entities.User.BulkSMSCount(RegionId: CurrentRegion.Id),
                Sent = false,
                FilterView = UserFilterView.All,
                CategoryId = 0
            };

            var Views = new List<EnumDescriptor>
            {
                new EnumDescriptor{ Id = (int)UserFilterView.All, Title = "All" },
                new EnumDescriptor{ Id = (int)UserFilterView.Active, Title = "Active" },
                new EnumDescriptor{ Id = (int)UserFilterView.EquipmentOwner, Title = "Equipment Owner" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedSearch, Title = "Completed a search" },
                new EnumDescriptor{ Id = (int)UserFilterView.MatchedSearch, Title = "Matched a search" },
                new EnumDescriptor{ Id = (int)UserFilterView.MadeBooking, Title = "Made a booking" },
                new EnumDescriptor{ Id = (int)UserFilterView.BookingConfirmed, Title = "Booking confirmed" },
                new EnumDescriptor{ Id = (int)UserFilterView.PaidBooking, Title = "Paid for booking" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedBooking, Title = "Booking completed" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedSearchNoMatch, Title = "Search no matches" },
                new EnumDescriptor{ Id = (int)UserFilterView.MatchedSearchNoBooking, Title = "Matched search no booking" }
            };

            var Categories = Category.List(ParentId: 0).Select(e => e.Json()).ToList();
            Categories.Insert(0, new { Id = 0, Title = "All" });
            
            var data = new
            {
                Entity = entity,
                Views,
                Categories
            };

            return Success(data);
        }

        [Route("sms/recipients/count")]
        [AcceptVerbs("GET")]
        public object RecipientCount(UserFilterView View, int CategoryId)
        {
            var count = -1;

            if (View == UserFilterView.All)
            {
                count = Entities.User.BulkSMSCount(RegionId: CurrentRegion.Id);
            }
            else
            {
                count = Entities.User.FilteredCount(FilterView: View, EquipmentCategoryId: CategoryId, RegionId: CurrentRegion.Id, BulkSms: true);
            }

            var entity = new SmsModel
            {
                RecipientCount = count
            };


            var data = new
            {
                Entity = entity
            };

            return Success(data);
        }

        [Route("sms/save")]
        [AcceptVerbs("POST")]
        public object Save(SmsModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            Model.Sent = true;

            var users = new List<string>();

            if (Model.FilterView == UserFilterView.All)
            {
                users = Entities.User
                    .BulkSMSList(RegionId: CurrentRegion.Id)
                    .Select(e => e.Telephone)
                    .ToList();
            }
            else
            {
                users = Entities.User
                    .FilteredList(FilterView: Model.FilterView, EquipmentCategoryId: Model.CategoryId, RegionId: CurrentRegion.Id, BulkSms: true)
                    .Select(e => e.Telephone)
                    .ToList();
            }

            if (Core.Utils.SMS.SendMessages(users, Model.Message, CurrentRegion))
                return Success(new
                {
                    Entity = Model,
                    Feedback = "Broadcast SMS sent"
                });

            return Error();
        }
    }

    public class SmsModel
    {
        public string Message { get; set; }
        public int RecipientCount { get; set; }
        public bool Sent { get; set; }
        public int UserId { get; set; }
        public UserFilterView FilterView { get; set; }
        public int CategoryId { get; set; }
    }
}
