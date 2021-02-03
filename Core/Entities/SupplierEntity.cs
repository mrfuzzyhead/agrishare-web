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
    public partial class Supplier : IEntity
    {
        public static decimal TransportCostPerKm => Convert.ToDecimal(Config.Find(Key: "Transport Cost Per KM")?.Value ?? "0");

        public static string DefaultSort = "Title";

        private File _logo;
        public File Logo
        {
            get
            {
                if (_logo == null && !LogoPath.IsEmpty())
                    _logo = new File(LogoPath);
                return _logo;
            }
            set
            {
                _logo = value;
                if (_logo != null)
                    LogoPath = _logo.Name + _logo.Extension;
            }
        }

        public static Supplier Find(int Id = 0)
        {
            if (Id == 0)
                return new Supplier
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Suppliers.Include(o => o.Region).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Supplier> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Suppliers.Include(o => o.Region).Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Suppliers.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var region = Region;
            if (region != null)
                RegionId = region.Id;
            Region = null;

            var logo = Logo;
            if (logo != null) LogoPath = logo.Name + logo.Extension;
            Logo = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Logo = logo;
            Region = region;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Suppliers.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Suppliers.Attach(this);
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

        public object TitleJson()
        {
            return new
            {
                Id,
                Title
            };
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                Region = Region?.Json(),
                Logo = Logo?.JSON(),
                Location,
                Latitude,
                Longitude,
                DateCreated,
                LastModified
            };
        }
    }
}
