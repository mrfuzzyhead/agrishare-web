using Agrishare.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class User : IEntity
    {
        public static bool VerificationRequired => Convert.ToBoolean(Config.Find(Key: "User Verification Required")?.Value ?? "True");

        public static int MaxFailedLoginAttempts = 0;
        public static int MaxFailedVoucherAttempts = 10;

        public static string AuthCookieName = "agrishare_pp";
        public static string DefaultSort = "FirstName";
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Title => FullName;
        public string ClearPassword { get; set; }
        public string Interest => $"{InterestId}".ExplodeCamelCase();
        public string Gender => $"{GenderId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public string Language => $"{LanguageId}".ExplodeCamelCase();
        public string AgentType => $"{AgentTypeId}".ExplodeCamelCase();

        private List<Role> roles { get; set; }
        public List<Role> Roles
        {
            get
            {
                if (roles == null || roles.Count == 0)
                    roles = RoleList?.Split(',').Where(e => !e.Trim().IsEmpty()).Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true)).ToList();
                if (roles == null)
                    roles = new List<Role>();
                return roles;
            }
            set
            {
                roles = value;
            }
        }

        public BankAccount bankAccount { get; set; }
        public BankAccount BankAccount
        {
            get
            {
                if (bankAccount == null && !string.IsNullOrEmpty(EncryptedBankAccountJson))
                {
                    var bankAccountJson = Utils.Encryption.DecryptWithRC4(EncryptedBankAccountJson);
                    bankAccount = JsonConvert.DeserializeObject<BankAccount>(bankAccountJson);
                }
                if (bankAccount == null)
                    bankAccount = new BankAccount();
                return bankAccount;
            }
            set
            {
                bankAccount = value;
            }
        }

        private List<PaymentMethod> paymentMethods;
        public List<PaymentMethod> PaymentMethods
        {
            get
            {
                if (paymentMethods == null)
                {
                    paymentMethods = new List<PaymentMethod>();
                    if ((PaymentMethod & (int)Entities.PaymentMethod.BankTransfer) > 0)
                        paymentMethods.Add(Entities.PaymentMethod.BankTransfer);
                    if ((PaymentMethod & (int)Entities.PaymentMethod.Cash) > 0)
                        paymentMethods.Add(Entities.PaymentMethod.Cash);
                    if ((PaymentMethod & (int)Entities.PaymentMethod.MobileMoney) > 0)
                        paymentMethods.Add(Entities.PaymentMethod.MobileMoney);
                }
                return paymentMethods;
            }
            set
            {
                paymentMethods = value;
            }
        }

        private static string CacheKey(string AuthToken)
        {
            return $"User:{AuthToken}";
        }

        public static User Find(int Id = 0, string EmailAddress = "", string Telephone = "", string AuthToken = "", bool Deleted = false)
        {
            if (!AuthToken.IsEmpty())
            {
                var item = Cache.Instance.Get<User>(CacheKey(AuthToken));

                if (item != null)
                {
                    // HACK to remove duplicates from roles list
                    item.Roles = item.Roles.Distinct().ToList();
                    return item;
                }
            }

            if (Id == 0 && EmailAddress.IsEmpty() && Telephone.IsEmpty() && AuthToken.IsEmpty())
                return new User {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users
                    .Include(o => o.Region)
                    .Include(o => o.Agent)
                    .Where(o => o.Deleted == Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!EmailAddress.IsEmpty())
                    query = query.Where(e => e.EmailAddress.Equals(EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                if (!Telephone.IsEmpty())
                    query = query.Where(e => e.Telephone.Equals(Telephone, StringComparison.InvariantCultureIgnoreCase));

                if (!AuthToken.IsEmpty())
                    query = query.Where(e => e.AuthToken.Equals(AuthToken));

                return query.FirstOrDefault();
            }
        }

        public static List<User> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "",
            string StartsWith = "", Gender Gender = Entities.Gender.None, int FailedLoginAttempts = 0,
            bool Deleted = false, int AgentId = 0, UserStatus Status = UserStatus.None, DateTime? RegisterFromDate = null,
            DateTime? RegisterToDate = null, bool? Agent = null, bool? Administrator = null, int RegionId = 0, int SupplierId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users
                    .Include(o => o.Region)
                    .Include(o => o.Agent)
                    .Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                if (Gender != Gender.None)
                    query = query.Where(o => o.GenderId == Gender);

                if (FailedLoginAttempts > 0)
                    query = query.Where(o => o.FailedLoginAttempts > 0);

                if (AgentId > 0)
                    query = query.Where(o => o.AgentId == AgentId);

                if (Status != UserStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                if (RegisterFromDate.HasValue)
                {
                    var fromDate = RegisterFromDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= fromDate);
                }
                if (RegisterToDate.HasValue)
                {
                    var toDate = RegisterToDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= toDate);
                }

                if (Agent.HasValue && Agent.Value == true)
                    query = query.Where(o => o.AgentId.HasValue);
                if (Agent.HasValue && Agent.Value == false)
                    query = query.Where(o => !o.AgentId.HasValue);

                if (Administrator.HasValue && Administrator.Value == true)
                    query = query.Where(o => o.RoleList.Contains("Administrator"));
                if (Administrator.HasValue && Administrator.Value == false)
                    query = query.Where(o => !o.RoleList.Contains("Administrator"));

                if (RegionId > 0)
                    query = query.Where(e => e.RegionId == RegionId);

                if (SupplierId > 0)
                    query = query.Where(e => e.SupplierId == SupplierId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", Gender Gender = Entities.Gender.None, int FailedLoginAttempts = 0,
            bool Deleted = false, int AgentId = 0, UserStatus Status = UserStatus.None, bool? Agent = null, DateTime? RegisterFromDate = null,
            DateTime? RegisterToDate = null, bool? Administrator = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                if (Gender != Gender.None)
                    query = query.Where(o => o.GenderId == Gender);

                if (FailedLoginAttempts > 0)
                    query = query.Where(o => o.FailedLoginAttempts > FailedLoginAttempts);

                if (AgentId > 0)
                    query = query.Where(o => o.AgentId == AgentId);

                if (Status != UserStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                if (Agent.HasValue && Agent.Value == true)
                    query = query.Where(o => o.AgentId.HasValue);
                if (Agent.HasValue && Agent.Value == false)
                    query = query.Where(o => !o.AgentId.HasValue);

                if (Administrator.HasValue && Administrator.Value == true)
                    query = query.Where(o => o.RoleList.Contains("Administrator"));
                if (Administrator.HasValue && Administrator.Value == false)
                    query = query.Where(o => !o.RoleList.Contains("Administrator"));

                if (RegisterFromDate.HasValue)
                {
                    var fromDate = RegisterFromDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= fromDate);
                }
                if (RegisterToDate.HasValue)
                {
                    var toDate = RegisterToDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= toDate);
                }

                if (RegionId > 0)
                    query = query.Where(e => e.RegionId == RegionId);

                return query.Count();
            }
        }

        public static List<User> BulkSMSList(int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == false && o.RegionId == RegionId && (o.NotificationPreferences & (int)Entities.NotificationPreferences.BulkSMS) > 0);
                return query.ToList();
            }
        }

        public static int BulkSMSCount(int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == false && o.RegionId == RegionId && (o.NotificationPreferences & (int)Entities.NotificationPreferences.BulkSMS) > 0);
                return query.Count();
            }
        }

        public static int FilteredCount(UserFilterView FilterView, string Keywords = "", string StartsWith = "", Gender Gender = Entities.Gender.None,
            int FailedLoginAttempts = 0, bool Deleted = false, int AgentId = 0, UserStatus Status = UserStatus.None, DateTime? FilterStartDate = null,
            DateTime? FilterEndDate = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                IQueryable<User> query = null;

                switch (FilterView)
                {
                    case UserFilterView.Active:
                        var counter1 = ctx.Counters.Include(c => c.User.Agent);
                        if (FilterStartDate.HasValue)
                            counter1 = counter1.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter1 = counter1.Where(c => c.DateCreated <= FilterEndDate);
                        query = counter1.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;

                    case UserFilterView.CompletedSearchNoMatch:
                    case UserFilterView.MatchedSearchNoBooking:

                        var counter3 = ctx.Counters.Include(c => c.User.Agent);

                        if (FilterStartDate.HasValue)
                            counter3 = counter3.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter3 = counter3.Where(c => c.DateCreated <= FilterEndDate);

                        var searchEvent = $"{Counters.Search}".ExplodeCamelCase();
                        var matchEvent = $"{Counters.Match}".ExplodeCamelCase();
                        var bookEvent = $"{Counters.Book}".ExplodeCamelCase();

                        switch (FilterView)
                        {
                            case UserFilterView.CompletedSearchNoMatch:
                                counter3 = counter3.Where(c => c.Event == searchEvent || c.Event == matchEvent);

                                query = counter3
                                    .GroupBy(c => c.UserId)
                                    .Where(c => c.Count(d => d.Event == matchEvent) == 0)
                                    .Select(g => g.FirstOrDefault().User)
                                    .Where(u => !u.Deleted);

                                break;

                            case UserFilterView.MatchedSearchNoBooking:
                                counter3 = counter3.Where(c => c.Event == matchEvent || c.Event == bookEvent);

                                query = counter3
                                    .GroupBy(c => c.UserId)
                                    .Where(c => c.Count(d => d.Event == bookEvent) == 0)
                                    .Select(g => g.FirstOrDefault().User)
                                    .Where(u => !u.Deleted);

                                break;
                        }

                        break;

                    case UserFilterView.CompletedSearch:
                    case UserFilterView.MatchedSearch:
                    case UserFilterView.MadeBooking:
                    case UserFilterView.BookingConfirmed:
                    case UserFilterView.PaidBooking:
                    case UserFilterView.CompletedBooking:

                        var eventName = "";
                        switch (FilterView)
                        {
                            case UserFilterView.CompletedSearch: eventName = $"{Counters.Search}".ExplodeCamelCase(); break;
                            case UserFilterView.MatchedSearch: eventName = $"{Counters.Match}".ExplodeCamelCase(); break;
                            case UserFilterView.MadeBooking: eventName = $"{Counters.Book}".ExplodeCamelCase(); break;
                            case UserFilterView.BookingConfirmed: eventName = $"{Counters.ConfirmBooking}".ExplodeCamelCase(); break;
                            case UserFilterView.PaidBooking: eventName = $"{Counters.CompletePayment}".ExplodeCamelCase(); break;
                            case UserFilterView.CompletedBooking: eventName = $"{Counters.CompleteBooking}".ExplodeCamelCase(); break;
                        }

                        var counter2 = ctx.Counters.Include(c => c.User.Agent).Where(c => c.Event == eventName);
                        if (FilterStartDate.HasValue)
                            counter2 = counter2.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter2 = counter2.Where(c => c.DateCreated <= FilterEndDate);
                        query = counter2.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;

                    case UserFilterView.EquipmentOwner:
                        var listings = ctx.Listings.Include(c => c.User.Agent).Where(l => !l.Deleted);
                        if (FilterStartDate.HasValue)
                        {
                            var startDate = FilterStartDate.Value.StartOfDay();
                            listings = listings.Where(c => c.DateCreated >= startDate);
                        }
                        if (FilterEndDate.HasValue)
                        {
                            var endDate = FilterEndDate.Value.StartOfDay();
                            listings = listings.Where(c => c.DateCreated <= endDate);
                        }
                        query = listings.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;
                }

                if (query == null)
                    query = ctx.Users.Include(o => o.Region).Include(o => o.Agent).Where(u => !u.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                if (Gender != Gender.None)
                    query = query.Where(o => o.GenderId == Gender);

                if (FailedLoginAttempts > 0)
                    query = query.Where(o => o.FailedLoginAttempts > 0);

                if (AgentId > 0)
                    query = query.Where(o => o.AgentId == AgentId);

                if (Status != UserStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                return query.Count();
            }
        }

        public static List<User> FilteredList(UserFilterView FilterView, int PageIndex = 0, int PageSize = int.MaxValue,
            string Sort = "", string Keywords = "", string StartsWith = "", Gender Gender = Entities.Gender.None, int FailedLoginAttempts = 0,
            bool Deleted = false, int AgentId = 0, UserStatus Status = UserStatus.None, DateTime? FilterStartDate = null,
            DateTime? FilterEndDate = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                IQueryable<User> query = null;

                switch (FilterView)
                {
                    case UserFilterView.Active:
                        var counter1 = ctx.Counters.Include(c => c.User.Agent);
                        if (FilterStartDate.HasValue)
                            counter1 = counter1.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter1 = counter1.Where(c => c.DateCreated <= FilterEndDate);
                        query = counter1.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;

                    case UserFilterView.CompletedSearchNoMatch:
                    case UserFilterView.MatchedSearchNoBooking:

                        var counter3 = ctx.Counters.Include(c => c.User.Agent);

                        if (FilterStartDate.HasValue)
                            counter3 = counter3.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter3 = counter3.Where(c => c.DateCreated <= FilterEndDate);

                        var searchEvent = $"{Counters.Search}".ExplodeCamelCase();
                        var matchEvent = $"{Counters.Match}".ExplodeCamelCase();
                        var bookEvent = $"{Counters.Book}".ExplodeCamelCase();

                        switch (FilterView)
                        {
                            case UserFilterView.CompletedSearchNoMatch:
                                counter3 = counter3.Where(c => c.Event == searchEvent || c.Event == matchEvent);

                                query = counter3
                                    .GroupBy(c => c.UserId)
                                    .Where(c => c.Count(d => d.Event == matchEvent) == 0)
                                    .Select(g => g.FirstOrDefault().User)
                                    .Where(u => !u.Deleted);

                                break;

                            case UserFilterView.MatchedSearchNoBooking:
                                counter3 = counter3.Where(c => c.Event == matchEvent || c.Event == bookEvent);

                                query = counter3
                                    .GroupBy(c => c.UserId)
                                    .Where(c => c.Count(d => d.Event == bookEvent) == 0)
                                    .Select(g => g.FirstOrDefault().User)
                                    .Where(u => !u.Deleted);

                                break;
                        }

                        break;

                    case UserFilterView.CompletedSearch:
                    case UserFilterView.MatchedSearch:
                    case UserFilterView.MadeBooking:
                    case UserFilterView.BookingConfirmed:
                    case UserFilterView.PaidBooking:
                    case UserFilterView.CompletedBooking:

                        var eventName = "";
                        switch (FilterView)
                        {
                            case UserFilterView.CompletedSearch: eventName = $"{Counters.Search}".ExplodeCamelCase(); break;
                            case UserFilterView.MatchedSearch: eventName = $"{Counters.Match}".ExplodeCamelCase(); break;
                            case UserFilterView.MadeBooking: eventName = $"{Counters.Book}".ExplodeCamelCase(); break;
                            case UserFilterView.BookingConfirmed: eventName = $"{Counters.ConfirmBooking}".ExplodeCamelCase(); break;
                            case UserFilterView.PaidBooking: eventName = $"{Counters.CompletePayment}".ExplodeCamelCase(); break;
                            case UserFilterView.CompletedBooking: eventName = $"{Counters.CompleteBooking}".ExplodeCamelCase(); break;
                        }

                        var counter2 = ctx.Counters.Include(c => c.User.Agent).Where(c => c.Event == eventName);
                        if (FilterStartDate.HasValue)
                            counter2 = counter2.Where(c => c.DateCreated >= FilterStartDate);
                        if (FilterEndDate.HasValue)
                            counter2 = counter2.Where(c => c.DateCreated <= FilterEndDate);
                        query = counter2.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;

                    case UserFilterView.EquipmentOwner:
                        var listings = ctx.Listings.Include(c => c.User.Agent).Where(l => !l.Deleted);
                        if (FilterStartDate.HasValue)
                        {
                            var startDate = FilterStartDate.Value.StartOfDay();
                            listings = listings.Where(c => c.DateCreated >= startDate);
                        }
                        if (FilterEndDate.HasValue)
                        {
                            var endDate = FilterEndDate.Value.StartOfDay();
                            listings = listings.Where(c => c.DateCreated <= endDate);
                        }
                        query = listings.GroupBy(c => c.UserId).Select(g => g.FirstOrDefault().User).Where(u => !u.Deleted);
                        break;
                }

                if (query == null)
                    query = ctx.Users.Include(o => o.Region).Include(o => o.Agent).Where(u => !u.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                if (Gender != Gender.None)
                    query = query.Where(o => o.GenderId == Gender);

                if (FailedLoginAttempts > 0)
                    query = query.Where(o => o.FailedLoginAttempts > 0);

                if (AgentId > 0)
                    query = query.Where(o => o.AgentId == AgentId);

                if (Status != UserStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public bool Save()
        {
            var success = false;

            if (!ClearPassword.IsEmpty())
            {
                Salt = Utils.Encryption.GetSalt();
                Password = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            }

            if (Roles != null)
                RoleList = string.Join(",", Roles);

            PaymentMethod = 0;
            if (PaymentMethods.Contains(Entities.PaymentMethod.BankTransfer))
                PaymentMethod += (int)Entities.PaymentMethod.BankTransfer;
            if (PaymentMethods.Contains(Entities.PaymentMethod.Cash))
                PaymentMethod += (int)Entities.PaymentMethod.Cash;
            if (PaymentMethods.Contains(Entities.PaymentMethod.MobileMoney))
                PaymentMethod += (int)Entities.PaymentMethod.MobileMoney;

            if (BankAccount != null)
                EncryptedBankAccountJson = Utils.Encryption.EncryptWithRC4(JsonConvert.SerializeObject(BankAccount));

            var agent = Agent;
            if (agent != null && agent.Id > 0)
                AgentId = agent.Id;
            Agent = null;

            var region = Region;
            if (region != null && region.Id > 0)
                RegionId = region.Id;
            Region = null;

            var supplier = Supplier;
            if (supplier != null && supplier.Id != 0)
                SupplierId = supplier.Id;
            Supplier = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Agent = agent;
            Region = region;
            Supplier = supplier;

            if (!AuthToken.IsEmpty())
                Cache.Instance.Add(CacheKey(AuthToken), this.AdminJson());

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Users.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Users.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            if (!AuthToken.IsEmpty())
                Cache.Instance.Remove(CacheKey(AuthToken));

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                FirstName,
                LastName,
                Telephone,
                Language,
                AgentId,
                Agent = Agent?.Json(),
                AgentTypeId,
                AgentType,
                Region = Region?.Json(),
                Supplier = Supplier?.TitleJson()
            };
        }

        public string ProfileJsonString()
        {
            return JsonConvert.SerializeObject(ProfileJson(), Formatting.None);
        }

        public string CmsJsonString()
        {
            return JsonConvert.SerializeObject(CmsJson(), Formatting.None);
        }

        public object CmsJson()
        {
            return new
            {
                Id,
                FirstName,
                LastName,
                AuthToken,
                Roles = Roles.Select(e => $"{e}".ExplodeCamelCase()).ToList(),
                AgentId,
                Agent = Agent?.Json(),
                AgentTypeId,
                AgentType,
                Status,
                StatusId,
                Telephone,
                Language,
                Region = Region?.Json()
            };
        }

        public object ProfileJson()
        {
            return new
            {
                Id,
                FirstName,
                LastName,
                EmailAddress,
                Telephone,
                DateOfBirth,
                GenderId,
                Gender,
                AuthToken,
                NotificationPreferences = new
                {
                    SMS = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.SMS) > 0,
                    PushNotifications = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.PushNotifications) > 0,
                    Email = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.Email) > 0,
                    BulkSMS = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.BulkSMS) > 0
                },
                InterestId,
                Interest,
                LanguageId,
                Language,
                AgentId,
                Agent = Agent?.Json(),
                AgentTypeId,
                AgentType,
                Region = Region?.Json(),
                BankAccount,
                PaymentMethods
            };
        }

        public object AdminJson()
        {
            return new
            {
                Id,
                FirstName,
                LastName,
                EmailAddress,
                Telephone,
                DateOfBirth,
                GenderId,
                Gender,
                AuthToken,
                FailedLoginAttempts,
                FailedVoucherAttempts,
                VerificationCode,
                VerificationCodeExpiry,
                NotificationPreferences,
                InterestId,
                Interest,
                StatusId,
                Status,
                LanguageId,
                Language,
                Roles,
                DateCreated,
                LastModified,
                AgentId,
                Agent = Agent?.Json(),
                AgentTypeId,
                AgentType,
                Region = Region?.Json(),
                BankAccount,
                PaymentMethods
            };
        }

        public static int CleanDeletedAccounts()
        {
            using (var ctx = new AgrishareEntities())
            {
                var oneYearAgo = DateTime.Now.AddYears(-10);
                var users = ctx.Users.Where(o => o.Deleted && o.LastModified > oneYearAgo);
                foreach(var user in users)
                    ctx.Entry(user).State = EntityState.Deleted;
                return ctx.SaveChanges();
            }
        }

        #region Authorisation

        public bool UniqueTelephone()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Users.Count(o => !o.Deleted && o.Id != Id && o.Telephone == Telephone) == 0;
        }

        public bool UniqueEmailAddress()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Users.Count(o => !o.Deleted && o.Id != Id && o.EmailAddress == EmailAddress) == 0;
        }

        public bool ValidatePassword(string ClearPassword)
        {
            string encryptedPassowrd = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            return encryptedPassowrd == Password;
        }

        public bool SendVerificationCode()
        {
            if (VerificationCode.IsEmpty() || VerificationCodeExpiry < DateTime.UtcNow)
                VerificationCode = GeneratePIN(4);

            VerificationCodeExpiry = DateTime.UtcNow.AddDays(1);
            Save();

            bool emailSent = false, smsSent = false;

            if (!string.IsNullOrEmpty(EmailAddress))
            {
                var template = Template.Find(Title: "Verification Code");
                template.Replace("Code", VerificationCode);
                template.Replace("Expiry Date", VerificationCodeExpiry.Value.ToString("h:mmtt d MMMM yyyy"));

                new Email
                {
                    Message = template.EmailHtml(),
                    RecipientEmail = EmailAddress,
                    SenderEmail = Config.ApplicationEmailAddress,
                    Subject = "Verification Code"
                }.Send();

                emailSent = true;
            }

            var message = string.Format(Translations.Translate(TranslationKey.VerificationCode, LanguageId), VerificationCode);
            smsSent = SMS.SendMessage(Telephone, message, Region);

            return emailSent || smsSent;
        }

        #endregion

        #region Utils

        private static Random random = new Random();

        public static string GeneratePIN(int length)
        {  
            var randomString = "";
            for (int i = 0; i < length; i++)
                randomString = randomString + (char)random.Next(48, 58);
            return randomString;
        }

        #endregion

        public static List<AgeGenderData> GetAgeGenderData(int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = $@"SELECT 
	                            CASE 
                                WHEN Age IS NULL THEN 0
	                            WHEN Age <= 17 THEN 0
	                            WHEN Age BETWEEN 18 AND 24 THEN 1
	                            WHEN Age BETWEEN 25 AND 34 THEN 2
	                            WHEN Age BETWEEN 35 AND 44 THEN 3
	                            WHEN Age BETWEEN 45 AND 54 THEN 4
	                            WHEN Age BETWEEN 55 AND 64 THEN 5
	                            WHEN Age >= 65 THEN 6
	                            END AS AgeRangeIndex,
	                            GenderId AS Gender,
	                            IFNULL(COUNT(GenderId), 0) AS `Count`
                            FROM (SELECT FLOOR(DATEDIFF(NOW(), DateOfBirth) / 365) AS Age, GenderId FROM Users WHERE RegionId = {RegionId}) AS `Data`                    
                            GROUP BY AgeRangeIndex, GenderId";

                return ctx.Database.SqlQuery<AgeGenderData>(sql).ToList();
            }
        }

        public class AgeGenderData
        {
            public string AgeRange => AgeRanges[AgeRangeIndex];
            public int AgeRangeIndex { get; set; }
            public Gender Gender { get; set; }
            public int Count { get; set; }

            public static List<string> AgeRanges = new List<string>
            {
                "13 - 17",
                "18 - 24",
                "25 - 34",
                "35 - 44",
                "45 - 54",
                "55 - 64",
                "65 - 99"
            };
        }
    }

    public enum UserFilterView
    {
        All = 0,
        Active = 21,
        EquipmentOwner = 22,
        Agent = 4,
        Administrator = 5,
        CompletedSearch = 11,
        MatchedSearch = 12,
        MadeBooking = 13,
        BookingConfirmed = 14,
        PaidBooking = 15,
        CompletedBooking = 16,
        MatchedSearchNoBooking = 51,
        CompletedSearchNoMatch = 52
    }

    public class BankAccount
    {
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }

}
