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
    public partial class Agent : IEntity
    {
        public static string DefaultSort = "Title";
        public string Type => $"{TypeId}".ExplodeCamelCase();
        public int BookingCount = 0;

        public static Agent Find(int Id = 0)
        {
            if (Id == 0)
                return new Agent {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Agents.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Agent> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Agents.Where(o => o.Deleted == Deleted);

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
                var query = ctx.Agents.Where(o => o.Deleted == Deleted);

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
                ctx.Agents.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Agents.Attach(this);
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
                Commission,
                TypeId,
                Type,
                Telephone,
                BookingCount,
                DateCreated
            };
        }

        public static List<AgentBookingCount> BookingCounts()
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = "SELECT users.agentid, COUNT(bookings.id) AS count FROM bookings INNER JOIN users ON bookings.userid = users.id WHERE users.agentid IS NOT NULL GROUP BY users.agentid";
                return ctx.Database.SqlQuery<AgentBookingCount>(sql).ToList();
            }
        }
    }

    public class AgentBookingCount
    {
        public int AgentId { get; set; }
        public int Count { get; set; }
    }
}
