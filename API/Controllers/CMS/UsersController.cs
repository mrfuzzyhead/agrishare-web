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
    public class UsersController : BaseApiController
    {
        [Route("users/me")]
        [AcceptVerbs("GET")]
        public object Me()
        {
            return Success(new
            {
                User = CurrentUser.Json()
            });
        }

        [Route("users/list")]
        [AcceptVerbs("GET")]
        public object List([FromUri] UserFilterModel Filter, int PageIndex = 0, int PageSize = 25)
        {
            int recordCount = 0;
            List<User> list = new List<User>();

            if ((int)Filter.View > 10)
            {
                recordCount = Entities.User.FilteredCount(FilterView: Filter.View, Keywords: Filter.Query, Gender: Filter.Gender, FilterStartDate: Filter.StartDate, FilterEndDate: Filter.EndDate, RegionId: CurrentRegion.Id);
                list = Entities.User.FilteredList(FilterView: Filter.View, PageIndex: PageIndex, PageSize: PageSize, Keywords: Filter.Query, Gender: Filter.Gender, FilterStartDate: Filter.StartDate, FilterEndDate: Filter.EndDate, RegionId: CurrentRegion.Id);
            }
            else
            {
                recordCount = Entities.User.Count(Keywords: Filter.Query, Gender: Filter.Gender, Agent: Filter.View == UserFilterView.Agent ? (bool?)true : null, Administrator: Filter.View == UserFilterView.Administrator ? (bool?)true : null, RegionId: CurrentRegion.Id, RegisterFromDate: Filter.StartDate, RegisterToDate: Filter.EndDate);
                list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Filter.Query, Gender: Filter.Gender, Agent: Filter.View == UserFilterView.Agent ? (bool?)true : null, Administrator: Filter.View == UserFilterView.Administrator ? (bool?)true : null, RegionId: CurrentRegion.Id, RegisterFromDate: Filter.StartDate, RegisterToDate: Filter.EndDate);
            }

            int total = 0, active = 0, inactive = 0, male = 0, female = 0, deleted = 0, lockedout = 0, unverified = 0, totalAgents = 0, totalRegular = 0;
            if (PageIndex == 0)
            {
                total = Entities.User.Count(RegionId: CurrentRegion.Id);
                totalAgents = Entities.User.Count(Agent: true, RegionId: CurrentRegion.Id);
                totalRegular = Entities.User.Count(Agent: false, RegionId: CurrentRegion.Id);
                male = Entities.User.Count(Gender: Gender.Male, RegionId: CurrentRegion.Id);
                female = Entities.User.Count(Gender: Gender.Female, RegionId: CurrentRegion.Id);
                deleted = Entities.User.Count(Deleted: true, RegionId: CurrentRegion.Id);
                lockedout = Entities.User.Count(FailedLoginAttempts: Entities.User.MaxFailedLoginAttempts, RegionId: CurrentRegion.Id);
                active = Entities.Counter.Count(UniqueUser: true, RegionId: CurrentRegion.Id);
                inactive = Entities.User.Count(RegionId: CurrentRegion.Id) - active;
                unverified = Entities.User.Count(Status: Entities.UserStatus.Pending, RegionId: CurrentRegion.Id);
            }

            var Genders = EnumInfo.ToList<Gender>().Where(g => g.Title != "None").ToList();
            Genders.Add(new EnumDescriptor { Id = 0, Title = "All" });

            var Views = new List<EnumDescriptor>
            {
                new EnumDescriptor{ Id = (int)UserFilterView.All, Title = "All" },
                new EnumDescriptor{ Id = (int)UserFilterView.Active, Title = "Active" },
                new EnumDescriptor{ Id = (int)UserFilterView.EquipmentOwner, Title = "Equipment Owner" },
                new EnumDescriptor{ Id = (int)UserFilterView.Agent, Title = "Agent" },
                new EnumDescriptor{ Id = (int)UserFilterView.Administrator, Title = "Administrator" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedSearch, Title = "Completed a search" },
                new EnumDescriptor{ Id = (int)UserFilterView.MatchedSearch, Title = "Matched a search" },
                new EnumDescriptor{ Id = (int)UserFilterView.MadeBooking, Title = "Made a booking" },
                new EnumDescriptor{ Id = (int)UserFilterView.BookingConfirmed, Title = "Booking confirmed" },
                new EnumDescriptor{ Id = (int)UserFilterView.PaidBooking, Title = "Paid for booking" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedBooking, Title = "Booking completed" },
                new EnumDescriptor{ Id = (int)UserFilterView.CompletedSearchNoMatch, Title = "Search no matches" },
                new EnumDescriptor{ Id = (int)UserFilterView.MatchedSearchNoBooking, Title = "Matched search no booking" }
            };

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.CmsJson()),
                Title = "Users",
                Genders,
                Views,
                Summary = new
                {
                    Total = total,
                    TotalAgents = totalAgents,
                    TotalRegular = totalRegular,
                    Active = active,
                    Inactive = inactive,
                    Male =  male,
                    Female = female,
                    Deleted = deleted,
                    LockedOut = lockedout,
                    Unverified = unverified
                }
            };

            return Success(data);
        }

        [Route("users/unverified/list")]
        [AcceptVerbs("GET")]
        public object UnverifiedList(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.User.Count(Keywords: Query, Status: UserStatus.Pending, RegionId: CurrentRegion.Id);
            var list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, Status: UserStatus.Pending, RegionId: CurrentRegion.Id);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.CmsJson()),
                Title = "Unverified Users"
            };

            return Success(data);
        }

        [Route("users/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var Suppliers = Entities.Supplier.List();
            Suppliers.Insert(0, new Supplier { Id = 0, Title = "None" });

            var data = new
            {
                Entity = Entities.User.Find(Id: Id).AdminJson(),
                Roles = EnumInfo.ToList<Entities.Role>(),
                Genders = EnumInfo.ToList<Entities.Gender>(),
                Languages = EnumInfo.ToList<Entities.Language>(),
                Regions = Region.List().Select(e => new { e.Id, e.Title }),
                Agents = Entities.Agent.List().Select(a => a.Json()),
                Statuses = EnumInfo.ToList<Entities.UserStatus>().Where(s => s.Id > 0),
                Types = EnumInfo.ToList<AgentUserType>(),
                Entities.User.MaxFailedLoginAttempts,
                Entities.User.MaxFailedVoucherAttempts,
                PaymentMethods = EnumInfo.ToList<PaymentMethod>().Where(e => e.Id != 0),
                Suppliers = Suppliers.Select(e => new { e.Id, e.Title })
            };

            return Success(data);
        }

        [Route("users/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.User User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (User.DateOfBirth.HasValue)
                User.DateOfBirth = User.DateOfBirth.Value.AddMinutes(UTCOffset);

            if (User.Id > 0)
            {
                var u = Entities.User.Find(Id: User.Id);
                User.Password = u.Password;
                User.Salt = u.Salt;
            }
            else
            {
                User.Region = CurrentRegion;
            }

            if (User.Save())
                return Success(new
                {
                    Entity = User.AdminJson()
                });

            return Error();
        }

        [Route("users/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var user = Entities.User.Find(Id: Id);

            var bookings = Entities.Booking.Count(UserId: user.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
                return Error("Can not delete account - user has upcoming bookings");

            var listings = Entities.Listing.List(UserId: user.Id);
            foreach (var listing in listings)
            {
                bookings = Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                    return Error("Can not delete account - user has upcoming bookings");
            }
            foreach (var listing in listings)
                listing.Delete();

            var devices = Entities.Device.List(UserId: user.Id);
            foreach (var device in devices)
                device.Delete();

            user.Delete();

            return Success(new
            {
                Entity = user.AdminJson()
            });
        }

        [Route("users/failedloginattempts/reset")]
        [AcceptVerbs("GET")]
        public object ResetFailedLoginAttempts(int Id)
        {
            var user = Entities.User.Find(Id: Id);
            user.FailedLoginAttempts = 0;
            if (user.Save())
                return Find(Id);
            return Error();
        }

        [Route("users/failedvoucherattempts/reset")]
        [AcceptVerbs("GET")]
        public object ResetFailedVoucherAttempts(int Id)
        {
            var user = Entities.User.Find(Id: Id);
            user.FailedVoucherAttempts = 0;
            if (user.Save())
                return Find(Id);
            return Error();
        }

        /* Password */

        [Route("users/password/find")]
        [AcceptVerbs("GET")]
        public object FindPassword(int Id = 0)
        {           
            var data = new
            {
                Entity = Entities.User.Find(Id: Id).AdminJson()
            };

            return Success(data);
        }

        [Route("users/password/save")]
        [AcceptVerbs("POST")]
        public object SavePassword(Entities.User User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var user = User.Find(Id: User.Id);
            user.ClearPassword = User.ClearPassword;
            user.FailedLoginAttempts = 0;

            if (User.Save())
                return Success(new
                {
                    Entity = User.AdminJson()
                });

            return Error();
        }

        /* Deleted */

        [Route("users/deleted/list")]
        [AcceptVerbs("GET")]
        public object DeletedList(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.User.Count(Keywords: Query, Deleted: true, RegionId: CurrentRegion.Id);
            var list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, Deleted: true, RegionId: CurrentRegion.Id);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Deleted Users"
            };

            return Success(data);
        }

        [Route("users/deleted/find")]
        [AcceptVerbs("GET")]
        public object FindDeleted(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.User.Find(Id: Id, Deleted: true).AdminJson(),
                Roles = EnumInfo.ToList<Entities.Role>(),
                Genders = EnumInfo.ToList<Entities.Gender>()
            };

            return Success(data);
        }

        /* SMS */

        [Route("users/sms/find")]
        [AcceptVerbs("GET")]
        public object SMSFind(int Id = 0)
        {
            var entity = new SmsModel
            {
                UserId = Id,
                RecipientCount = 1,
                Sent = false
            };

            var data = new
            {
                Entity = entity
            };

            return Success(data);
        }

        [Route("users/sms/save")]
        [AcceptVerbs("POST")]
        public object SMSSave(SmsModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var user = Entities.User.Find(Model.UserId);
            if (Core.Utils.SMS.SendMessage(user.Telephone, Model.Message, user.Region))
            {
                Model.Sent = true;

                return Success(new
                {
                    Entity = Model,
                    Feedback = "SMS sent"
                });
            }

            return Error();
        }
    }
}
