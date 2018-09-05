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
        public static string DefaultSort = "FirstName";
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Title => FullName;
        public string ClearPassword { get; set; }
        public string Interest => $"{InterestId}".ExplodeCamelCase();
        public string Gender => $"{GenderId}".ExplodeCamelCase();
        public string Status => $"{Status}".ExplodeCamelCase();
        public List<Role> Roles => RoleList.Split(',').Where(e => !e.Trim().IsEmpty()).Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true)).ToList();

        public static User Find(int Id = 0, string EmailAddress = "", string AuthToken = "", string PasswordResetToken = "")
        {
            if (Id == 0 && EmailAddress.IsEmpty() && AuthToken.IsEmpty() && PasswordResetToken.IsEmpty())
                return new User {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!EmailAddress.IsEmpty())
                    query = query.Where(e => e.EmailAddress.Equals(EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                if (!AuthToken.IsEmpty())
                    query = query.Where(e => e.AuthToken.Equals(AuthToken));

                if (!PasswordResetToken.IsEmpty())
                    query = query.Where(e => e.PasswordResetToken.Equals(PasswordResetToken));

                return query.FirstOrDefault();
            }
        }

        public static List<User> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            return success;
        }

        private bool Add()
        {
            if (!ClearPassword.IsEmpty())
            {
                Salt = Utils.Encryption.GetSalt();
                Password = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            }

            using (var ctx = new AgrishareEntities())
            {
                ctx.Users.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            if (!ClearPassword.IsEmpty())
            {
                Salt = Utils.Encryption.GetSalt();
                Password = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            }

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

            Deleted = true;
            return Update();
        }

        public object Json(bool Admin = false)
        {
            if (Admin)
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
                    PasswordResetToken,
                    FailedLoginAttempts,
                    VerificationCode,
                    VerificationCodeExpiry,
                    NotificationPreferences,
                    InterestId,
                    Interest,
                    FacebookId,
                    GoogleId,
                    AvatarUrl,
                    StatusId,
                    Status,
                    Roles,
                    DateCreated,
                    LastModified
                };

            return new
            {
                Id,
                FirstName,
                LastName,
                EmailAddress,
                Telephone                    
            };
        }

    }
}
