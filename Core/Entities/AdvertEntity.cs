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
    public partial class Advert : IEntity
    {
        public static string DefaultSort = "Title";

        private File banner;
        public File Banner
        {
            get
            {
                if (banner == null && !PhotoPath.IsEmpty())
                    banner = new File(PhotoPath);
                return banner;
            }
            set
            {
                banner = value;
                if (banner != null)
                    PhotoPath = banner.Name + banner.Extension;
            }
        }

        public static Advert Find(int Id = 0)
        {
            if (Id == 0)
                return new Advert {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Adverts.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Advert> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false, bool? Live = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Adverts.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (Live.HasValue)
                    if (Live.Value)
                        query = query.Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now);
                    else
                        query = query.Where(e => e.StartDate > DateTime.Now || e.EndDate < DateTime.Now);

                if (RegionId > 0)
                    query = query.Where(e => e.RegionId == RegionId);

                if (Sort == "Random")
                    return query.OrderBy(e => Guid.NewGuid().ToString()).Skip(PageIndex * PageSize).Take(PageSize).ToList();
                else
                    return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false, bool? Live = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Adverts.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (Live.HasValue)
                    if (Live.Value)
                        query = query.Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now);
                    else
                        query = query.Where(e => e.StartDate > DateTime.Now || e.EndDate < DateTime.Now);

                if (RegionId > 0)
                    query = query.Where(e => e.RegionId == RegionId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var banner = Banner;
            if (banner != null) PhotoPath = banner.Name + banner.Extension;
            Banner = null;

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
                ctx.Adverts.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Adverts.Attach(this);
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
                Banner = Banner?.JSON(),
                LinkUrl,
                ImpressionCount,
                ClickCount,
                StartDate,
                EndDate,
                RegionId,
                DateCreated
            };
        }

        public object AppJson()
        {
            return new
            {
                Id,
                Banner = $"{Config.CDNURL}/{Banner.Filename}",
                LinkUrl
            };
        }

        public static void AddImpression(int Id)
        {
            using (var ctx = new AgrishareEntities())
                ctx.Database.ExecuteSqlCommand($"UPDATE Adverts SET ImpressionCount = ImpressCount + 1 WHERE Id = {Id}");
        }

        public static void AddImpressions(List<int> Id)
        {
            if (Id.Count > 0)
                using (var ctx = new AgrishareEntities())
                {
                    var sql = string.Join(";", Id.Select(e => $"UPDATE Adverts SET ImpressionCount = ImpressionCount + 1 WHERE Id = {e}"));
                    ctx.Database.ExecuteSqlCommand(sql);
                }
        }

        public static void AddClick(int Id)
        {
            using (var ctx = new AgrishareEntities())
                ctx.Database.ExecuteSqlCommand($"UPDATE Adverts SET ClickCount = ClickCount + 1 WHERE Id = {Id}");
        }
    }
}
