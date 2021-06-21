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
    public partial class BookingTag : IEntity
    {
        public static string DefaultSort = "Id";
        public string Title => Tag?.Title ?? string.Empty;

        public static BookingTag Find(int Id = 0)
        {
            if (Id == 0)
                return new BookingTag {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingTags.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static BookingTag Find(int TagId, int BookingId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingTags.Where(o => o.TagId == TagId && o.BookingId == BookingId && o.Deleted == false);
                return query.FirstOrDefault();
            }
        }

        public static List<BookingTag> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false, int BookingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingTags.Where(o => o.Deleted == Deleted);

                if (BookingId > 0)
                    query = query.Where(e => e.BookingId == BookingId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false, int BookingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingTags.Where(o => o.Deleted == Deleted);

                if (BookingId > 0)
                    query = query.Where(e => e.BookingId == BookingId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                return query.Count();
            }
        }

        public static bool Add(int TagId, int BookingId)
        {
            var bookingTag = Find(TagId, BookingId);
            if (bookingTag == null)
                new BookingTag
                {
                    TagId = TagId,
                    BookingId = BookingId
                }.Save();

            var booking = Booking.Find(Id: BookingId);
            booking.Tags = Tag.BookingList(booking.Id);
            return booking.Save();
        }

        public static bool Remove(int TagId, int BookingId)
        {
            var bookingTag = Find(TagId, BookingId);
            if (bookingTag != null)
                bookingTag.Delete();

            var booking = Booking.Find(Id: BookingId);
            booking.Tags = Tag.BookingList(booking.Id);
            return booking.Save();
        }

        private bool Save()
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
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingTags.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingTags.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Update();
        }
    }
}
