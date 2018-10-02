﻿using Agrishare.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Listing : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Condition => $"{ConditionId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public List<File> Photos => PhotoPaths?.Split(',').Select(e => new File(e)).ToList();

        private Category _category;
        public Category Category
        {
            get
            {
                if (_category == null)
                    _category = Category.Find(CategoryId);
                return _category;
            }
            set
            {
                _category = value;
                if (value != null)
                    CategoryId = value.Id;
            }
        }

        public static Listing Find(int Id = 0)
        {
            if (Id == 0)
                return new Listing
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Include(o => o.Services).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Listing> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", 
            string Keywords = "", string StartsWith = "", int UserId = 0, int CategoryId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (CategoryId > 0)
                    query = query.Where(o => o.CategoryId == CategoryId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int UserId = 0, int CategoryId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (CategoryId > 0)
                    query = query.Where(o => o.CategoryId == CategoryId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var services = Services;
            Services = null;

            var category = Category;
            if (category != null)
                CategoryId = category.Id;
            Category = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Category = category;

            if (success)
            {
                var current = Service.List(ListingId: Id);

                var add = services.Except(current);
                foreach(var item in add)
                {
                    item.ListingId = Id;
                    item.Save();
                }

                var remove = current.Except(services);
                foreach (var item in remove)
                    item.Delete();
            }

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Listings.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Listings.Attach(this);
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
                UserId,
                Category = Category?.Json(),
                Title,
                Description,
                Location,
                Latitude,
                Longitude,
                Brand,
                HorsePower,
                Year,
                ConditionId,
                Condition,
                GroupServices,
                Photos = Photos?.Select(e => e.JSON()),
                AverageRating,
                RatingCount,
                Services = Services?.Select(e => e.Json()),
                StatusId,
                Status,
                DateCreated
            };
        }
    }
}