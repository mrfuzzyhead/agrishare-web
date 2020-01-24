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
    public partial class TransactionFee : IEntity
    {
        public static string DefaultSort = "MinimumValue";
        public string Title => $"${MinimumValue.ToString("N2")} - ${MaximumValue.ToString("N2")}";

        public static TransactionFee Find(int Id = 0)
        {
            if (Id == 0)
                return new TransactionFee {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.TransactionFees.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static TransactionFee Find(decimal Amount)
        {
            using (var ctx = new AgrishareEntities())
                return ctx.TransactionFees.FirstOrDefault(o => o.Deleted == false && o.MinimumValue <= Amount && o.MaximumValue >= Amount);
        }

        public static List<TransactionFee> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.TransactionFees.Where(o => o.Deleted == Deleted);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.TransactionFees.Where(o => o.Deleted == Deleted);

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
                ctx.TransactionFees.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.TransactionFees.Attach(this);
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

        public bool UniqueRange()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.TransactionFees.Count(f => f.MinimumValue <= MaximumValue && f.MaximumValue >= MinimumValue && f.Id != Id) == 0;
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                MinimumValue,
                MaximumValue,
                Fee,
                FeeType,
                FeeTypeDescription = $"{FeeType}".ExplodeCamelCase(),
                DateCreated
            };
        }
    }
}
