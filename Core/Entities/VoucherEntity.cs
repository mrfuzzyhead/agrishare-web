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
    public partial class Voucher : IEntity
    {
        public static string DefaultSort = "Title";

        public static Voucher Find(int Id = 0, string Code = "")
        {
            if (Id == 0 && string.IsNullOrEmpty(Code))
                return new Voucher
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Vouchers.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!string.IsNullOrEmpty(Code))
                    query = query.Where(e => e.Code.Equals(Code, StringComparison.InvariantCultureIgnoreCase));

                return query.FirstOrDefault();
            }
        }

        public static List<Voucher> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Vouchers.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Vouchers.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

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
            using (var ctx = new AgrishareEntities())
            {
                ctx.Vouchers.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Vouchers.Attach(this);
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

        public bool UniqueCode()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Vouchers.Count(e => !e.Deleted && e.Code == Code && e.Id != Id) == 0;
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                TypeId,
                Code,
                Amount,
                RedeemCount,
                MaxRedeemCount,
                DateCreated,
                LastModified
            };
        }
    }
}
