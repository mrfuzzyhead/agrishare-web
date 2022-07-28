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
    public partial class Faq : IEntity
    {
        public static string DefaultSort = "SortOrder";
        public string Title => Question;

        private List<Region> _regions;
        public List<Region> Regions
        {
            get
            {
                if (_regions == null && !string.IsNullOrEmpty(RegionsJson))
                    _regions = JsonConvert.DeserializeObject<List<Region>>(RegionsJson);
                if (_regions == null)
                    _regions = new List<Region>();
                return _regions;
            }
            set
            {
                _regions = value;
            }
        }

        public static Faq Find(int Id = 0)
        {
            if (Id == 0)
                return new Faq
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Faq> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", Language? Language = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Question + " " + o.Answer).ToLower().Contains(Keywords.ToLower()));

                if (Language.HasValue)
                    query = query.Where(o => o.LanguageId == Language.Value);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", Language? Language = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Question + " " + o.Answer).ToLower().Contains(Keywords.ToLower()));

                if (Language.HasValue)
                    query = query.Where(o => o.LanguageId == Language.Value);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            RegionsJson = JsonConvert.SerializeObject(Regions.Select(e => e.FaqsJson()));

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
                try { SortOrder = ctx.Faqs.Max(o => o.SortOrder) + 1; }
                catch { SortOrder = 1; }

                ctx.Faqs.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Faqs.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool UpdateOrder()
        {
            var success = false;

            using (var ctx = new AgrishareEntities())
            {
                if (SortOrder > 0)
                    ctx.Faqs.Where(o => o.SortOrder >= SortOrder).ToList().ForEach(o => { o.SortOrder += 1; });
                success = ctx.SaveChanges() > 0;
            }

            using (var ctx = new AgrishareEntities())
            {
                ctx.Faqs.Attach(this);
                if (SortOrder == 0)
                    SortOrder = ctx.Faqs.Max(o => o.SortOrder) + 1;
                ctx.Entry(this).State = EntityState.Modified;
                success = ctx.SaveChanges() > 0;
            }

            return success;
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
                Question,
                Answer,
                LanguageId,
                Regions = Regions.Select(e => e.FaqsJson()),
                SortOrder,
                DateCreated,
                LastModified
            };
        }
    }
}
