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
    public enum Regions: int
    {
        Zimbabwe = 1,
        Uganda = 2
    }

    public partial class Region : IEntity
    {
        public static string DefaultSort = "Title";

        public static Region Find(int Id = 0)
        {
            if (Id == 0)
                return new Region {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Regions.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Region> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Regions.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Regions.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

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
                ctx.Regions.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Regions.Attach(this);
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
                Currency,
                CurrencyCode = $"{Currency}"
            };
        }

        public string CmsJsonString()
        {
            return JsonConvert.SerializeObject(Json(), Formatting.None);
        }

        public static string CmsListJsonString()
        {
            var list = List().Select(e => e.Json());
            return JsonConvert.SerializeObject(list, Formatting.None);
        }
    }
}
