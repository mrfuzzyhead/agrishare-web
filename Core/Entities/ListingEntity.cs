using Agrishare.Core.Utils;
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
        public string UrlPath => $"/listing/{Id}/{Title.UrlPath()}";

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

        public static Listing Find(int Id = 0, bool Deleted = false)
        {
            if (Id == 0)
                return new Listing
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Include(o => o.Services).Include(o => o.User).Where(o => o.Deleted == Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Listing> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", 
            string Keywords = "", string StartsWith = "", int UserId = 0, int CategoryId = 0, ListingStatus Status = ListingStatus.None, bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (CategoryId > 0)
                    query = query.Where(o => o.CategoryId == CategoryId);

                if (Status != ListingStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int UserId = 0, int CategoryId = 0, ListingStatus Status = ListingStatus.None, bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Listings.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (CategoryId > 0)
                    query = query.Where(o => o.CategoryId == CategoryId);

                if (Status != ListingStatus.None)
                    query = query.Where(o => o.StatusId == Status);

                return query.Count();
            }
        }

        public static List<Listing> MapList(double NorthEastLatitude, double NorthEastLongitude, double SouthWestLatitude, double SouthWestLongitude, List<int> CategoryIds)
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = $"SELECT *, Photos AS PhotoPaths FROM Listings WHERE Listings.Deleted = 0 AND (Latitude BETWEEN {SouthWestLatitude} AND {NorthEastLatitude}) ";
                if (NorthEastLongitude < SouthWestLongitude)
                    sql += $"AND NOT (Longitude BETWEEN {NorthEastLongitude} AND {SouthWestLongitude})";
                else
                    sql += $"AND (Longitude BETWEEN {SouthWestLongitude} AND {NorthEastLongitude})";
                if (CategoryIds.Count > 0)
                    sql += $"AND Listings.CategoryId IN ({String.Join(",", CategoryIds)})";
                else
                    sql += $"AND Listings.CategoryId = -1";

                return ctx.Database.SqlQuery<Listing>(sql).ToList();
            }
        }

        public bool Save()
        {
            var success = false;

            var services = Services;
            Services = null;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            var category = Category;
            if (category != null)
                CategoryId = category.Id;
            Category = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Category = category;
            User = user;

            if (success)
            {
                var current = Service.List(ListingId: Id);
                var remove = current.Where(e => !services.Any(s => s.Id == e.Id));
                foreach (var item in remove)
                    item.Delete();

                foreach (var item in services)
                {
                    item.ListingId = Id;
                    item.Save();
                }
            }

            return success;
        }

        private bool Add()
        {
            StatusId = ListingStatus.Live;

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

        public object Json(bool IncludeUser = false)
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
                AvailableWithoutFuel,
                AvailableWithFuel,
                Photos = Photos?.Select(e => e.JSON()),
                AverageRating,
                RatingCount,
                Services = Services?.Select(e => e.Json()),
                StatusId,
                Status,
                User = IncludeUser ? User?.Json() : null,
                Url = $"{Config.WebURL}{UrlPath}",
                DateCreated,
                LastModified
            };
        }

        /* Reports */

        public static List<ListingData> Graph(DateTime StartDate, DateTime EndDate, int CategoryId = 0)
        {
            var sql = $@"SELECT MONTH(Listings.DateCreated) AS `Month`, YEAR(Listings.DateCreated) AS `Year`, COUNT(Listings.Id) AS 'Count' 
                            FROM Listings
                            INNER JOIN Categories ON Listings.CategoryId = Categories.Id
                            WHERE DATE(Listings.DateCreated) <= DATE('{EndDate.ToString("yyyy-MM-dd")}') AND 
	                            DATE(Listings.DateCreated) >= DATE('{StartDate.ToString("yyyy-MM-dd")}') ";

            if (CategoryId > 0)
                sql += $"AND Listings.CategoryId = {CategoryId} ";

            sql += $@"GROUP BY MONTH(Listings.DateCreated), YEAR(Listings.DateCreated) ORDER BY MONTH(Listings.DateCreated), YEAR(Listings.DateCreated) LIMIT 6";

            using (var ctx = new AgrishareEntities())
                return ctx.Database.SqlQuery<ListingData>(sql).ToList();
        }
    }

    public class ListingData
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }
    }
}
