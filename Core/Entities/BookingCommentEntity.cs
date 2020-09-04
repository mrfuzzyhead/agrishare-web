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
    public partial class BookingComment : IEntity
    {
        public static string DefaultSort = "DateCreated";
        public string Title => Text;

        public static BookingComment Find(int Id = 0)
        {
            if (Id == 0)
                return new BookingComment {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingComments.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<BookingComment> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false, int BookingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingComments.Include(e => e.User).Where(o => o.Deleted == Deleted);

                if (BookingId > 0)
                    query = query.Where(e => e.BookingId == BookingId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Text.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Text.ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false, int BookingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingComments.Include(e => e.User).Where(o => o.Deleted == Deleted);

                if (BookingId > 0)
                    query = query.Where(e => e.BookingId == BookingId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Text.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Text.ToLower().StartsWith(Keywords.ToLower()));

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var booking = Booking;
            Booking = null;
            BookingId = booking?.Id ?? BookingId;

            var user = User;
            User = null;
            UserId = user?.Id ?? UserId;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;
            User = user;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingComments.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingComments.Attach(this);
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

        public object Json()
        {
            return new
            {
                Id,
                Title,
                BookingId,
                User = User?.Json(),
                Text,
                DateCreated
            };
        }
    }
}
