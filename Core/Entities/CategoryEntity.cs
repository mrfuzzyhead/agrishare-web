﻿using Agrishare.Core.Utils;
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
    public partial class Category : IEntity
    {
        public const int TractorsId = 1;
        public const int LorriesId = 2;
        public const int LorriesServiceId = 16;
        public const int TractorTransportServiceId = 18;
        public const int ProcessingId = 3;
        public const int BusId = 17;
        public const int BusServiceId = 16;
        public const int IrrigationId = 60;
        public const int LabourId = 50;
        public const int LandId = 70;

        public static string DefaultSort = "Title";

        private static string CacheKey(int Id)
        {
            return $"Category:{Id}";
        }

        private List<Translation> _translations;
        public List<Translation> Translations
        {
            get
            {
                if (_translations == null && !string.IsNullOrEmpty(TranslationsJson))
                    _translations = JsonConvert.DeserializeObject<List<Translation>>(TranslationsJson);
                if (_translations == null)
                    _translations = new List<Translation>();
                return _translations;
            }
            set
            {
                _translations = value;
            }
        }

        public static Category Find(int Id = 0)
        {
            if (Id > 0)
            {
                var item = Cache.Instance.Get<Category>(CacheKey(Id));
                if (item != null)
                    return item;
            }

            if (Id == 0)
                return new Category
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Categories.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                var category = query.FirstOrDefault();

                if (category != null)
                    Cache.Instance.Add(CacheKey(Id), category);

                return category;
            }
        }

        public static List<Category> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", int? ParentId = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Categories.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (ParentId.HasValue)
                    if (ParentId == 0)
                        query = query.Where(e => e.ParentId == null);
                    else
                        query = query.Where(e => e.ParentId == ParentId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static List<Category> ParentListByListingCount()
        {
            using (var ctx = new AgrishareEntities())
            {
                var idList = ctx.Listings
                    .GroupBy(e => e.CategoryId)
                    .Select(e => new { e.FirstOrDefault().CategoryId, Count = e.Count() })
                    .OrderByDescending(e => e.Count)
                    .Select(e => e.CategoryId);

                var categories = ctx.Categories.Where(e => !e.ParentId.HasValue && !e.Deleted).ToList();

                var list = new List<Category>();
                foreach (var id in idList)
                    list.Add(categories.FirstOrDefault(e => e.Id == id));

                return list;
            }
        }

        public static int Count(string Keywords = "", int ParentId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Categories.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (ParentId == 0)
                    query = query.Where(e => e.ParentId == null);
                else
                    query = query.Where(e => e.ParentId == ParentId);

                return query.Count();
            }
        }

        public bool Save()
        {
            TranslationsJson = JsonConvert.SerializeObject(Translations);

            bool success;
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
                ctx.Categories.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Categories.Attach(this);
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
                Title
            };
        }

        public object AppJson()
        {
            return new
            {
                Id,
                Title,
                Translations = Translations.Select(e => new { e.Language, e.Text })
            };
        }
    }
}
