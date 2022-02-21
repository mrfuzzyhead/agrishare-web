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
    public partial class Journal : IEntity
    {
        public static decimal CurrentRate => Convert.ToDecimal(Config.Find(Key: "Current USD Rate").Value);

        public static string DefaultSort = "Date DESC";
        public string Type => $"{TypeId}".ExplodeCamelCase();
        public decimal Balance { get; set; }
        public decimal Debit => Amount < 0 ? Math.Abs(Amount) : 0;
        public decimal Credit => Amount > 0 ? Math.Abs(Amount) : 0;

        public static Journal Find(int Id = 0)
        {
            if (Id == 0)
                return new Journal
                {
                    Date = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals
                    .Include(o => o.User)
                    .Include(o => o.Region)
                    .Include(o => o.Booking)
                    .Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Journal> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int BookingId = 0, int UserId = 0, 
            DateTime? StartDate = null, DateTime? EndDate = null, JournalType Type = JournalType.None, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals
                    .Include(o => o.User)
                    .Include(o => o.Booking)
                    .Include(o => o.Region)
                    .Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= endDate);
                }

                if (Type != JournalType.None)
                    query = query.Where(e => e.TypeId == Type);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int BookingId = 0, int UserId = 0, DateTime? StartDate = null, DateTime? EndDate = null, 
            JournalType Type = JournalType.None, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (RegionId > 0)
                    query = query.Where(o => o.RegionId == RegionId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= endDate);
                }

                if (Type != JournalType.None)
                    query = query.Where(e => e.TypeId == Type);

                return query.Count();
            }
        }

        public static decimal BalanceAt(DateTime Date, int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);
                query = query.Where(o => o.Date <= Date);
                query = query.Where(o => o.RegionId == RegionId);
                try
                {
                    return query.Select(o => o.Amount).DefaultIfEmpty(0).Sum();
                }
                catch
                {
                    return 0;
                }
            }

        }

        public static decimal BalanceAt(int Id, int RegionId)
        {
            if (Id == 0)
                return 0;

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);
                query = query.Where(o => o.Id <= Id);
                query = query.Where(o => o.RegionId == RegionId);
                try
                {
                    return query.Select(o => o.Amount).DefaultIfEmpty(0).Sum();
                }
                catch
                {
                    return 0;
                }
            }
        }



        public static decimal Total(DateTime StartDate, DateTime EndDate, JournalType Type, int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();

                var query = ctx.Journals
                    .Where(o => !o.Deleted)
                    .Where(o => o.Date >= StartDate)
                    .Where(o => o.Date <= EndDate)
                    .Where(o => o.RegionId == RegionId);

                if (Type != JournalType.None)
                    query = query.Where(e => e.TypeId == Type);

                try
                {
                    return query.Select(o => o.Amount).DefaultIfEmpty(0).Sum();
                }
                catch
                {
                    return 0;
                }
            }

        }

        public bool Save()
        {
            var success = false;

            if (Currency == Currency.None && Region != null)
                Currency = Region.Currency;

            var booking = Booking;
            if (booking != null)
                BookingId = booking.Id;
            Booking = null;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            var region = Region;
            if (region != null)
                RegionId = region.Id;
            Region = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;
            User = user;
            Region = region;

            return success;
        }

        private bool Add()
        {
            if (!UniqueEcoCashReference())
                return false;

            using (var ctx = new AgrishareEntities())
            {
                ctx.Journals.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Journals.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool UniqueEcoCashReference()
        {
            if (string.IsNullOrEmpty(EcoCashReference))
                return true;

            using (var ctx = new AgrishareEntities())
                return ctx.Journals.Count(j => j.Id != Id && !j.Deleted && j.EcoCashReference == EcoCashReference) == 0;
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
                User = User?.Json(),
                Booking = Booking?.Json(),
                Region = Region?.Json(),
                Amount,
                Credit,
                Debit,
                CurrencyAmount,
                Currency,
                CurrencyCode = $"{Currency}".ExplodeCamelCase(),
                Reconciled,
                EcoCashReference,
                TypeId,
                Type,
                Date,
                DateCreated,
                LastModified,
                Deleted,
                Balance
            };
        }

        public static List<GraphData> Graph(DateTime StartDate, DateTime EndDate, GraphView View, int RegionId)
        {
            var sql = string.Empty;

            if (View == GraphView.Day)
            {
                sql = $@"SELECT DAY(Journals.Date) AS `Day`, MONTH(Journals.Date) AS `Month`, YEAR(Journals.Date) AS `Year`, SUM(Journals.Amount) AS 'Amount' 
                            FROM Journals
                            WHERE Journals.RegionId = {RegionId} AND DATE(Journals.Date) <= DATE('{EndDate.ToString("yyyy-MM-dd")}') AND DATE(Journals.Date) >= DATE('{StartDate.ToString("yyyy-MM-dd")}') 
                            GROUP BY DAY(Journals.Date), MONTH(Journals.Date), YEAR(Journals.Date) ORDER BY YEAR(Journals.Date), MONTH(Journals.Date), DAY(Journals.Date)";
            }
            else if (View == GraphView.Week)
            {
                sql = $@"SELECT WEEK(Journals.Date) AS `Week`, MONTH(Journals.Date) AS `Month`, YEAR(Journals.Date) AS `Year`, SUM(Journals.Amount) AS 'Amount' 
                            FROM Journals
                            WHERE Journals.RegionId = {RegionId} AND DATE(Journals.Date) <= DATE('{EndDate.ToString("yyyy-MM-dd")}') AND DATE(Journals.Date) >= DATE('{StartDate.ToString("yyyy-MM-dd")}') 
                            GROUP BY WEEK(Journals.Date), YEAR(Journals.Date) ORDER BY YEAR(Journals.Date), WEEK(Journals.Date)";
            }
            else
            {
                sql = $@"SELECT MONTH(Journals.Date) AS `Month`, MONTH(Journals.Date) AS `Month`, YEAR(Journals.Date) AS `Year`, SUM(Journals.Amount) AS 'Amount' 
                            FROM Journals
                            WHERE Journals.RegionId = {RegionId} AND DATE(Journals.Date) <= DATE('{EndDate.ToString("yyyy-MM-dd")}') AND DATE(Journals.Date) >= DATE('{StartDate.ToString("yyyy-MM-dd")}') 
                            GROUP BY MONTH(Journals.Date), YEAR(Journals.Date) ORDER BY YEAR(Journals.Date), MONTH(Journals.Date)";
            }

            using (var ctx = new AgrishareEntities())
                return ctx.Database.SqlQuery<GraphData>(sql).ToList();
        }

        public enum GraphView
        {
            Day = 1,
            Week = 2,
            Month = 3
        }

        public class GraphData
        {
            public int Day { get; set; }
            public int Week { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public decimal Amount { get; set; }
        }
    }

    public partial class JournalImport : ImportExcel
    {
        private List<Booking> Bookings;
        public Region Region;

        public JournalImport()
        {
            Bookings = Booking.List(Status: BookingStatus.Complete);
        }

        public override bool ValidateCell(int Column, string Data, out string Error)
        {
            Error = string.Empty;

            switch (Column)
            {
                case 1: // Date
                    Error += Required(Data);
                    Error += Date(Data);
                    break;
                case 2: // Booking ID
                    if (!Data.IsEmpty())
                    {
                        Error = Integer(Data);
                        if (Bookings.Where(o => o.Id == Convert.ToInt32(Data)).Count() == 0)
                            Error = "Booking does not exist";
                    }
                    break;
                case 3: // Supplier Name
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
                case 4: // Supplier Telephone
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
                case 5: // Amount
                    Error += Required(Data);
                    Error += Decimal(Data);
                    break;
                case 6: // Type (Agent or Supplier)
                    Error += Required(Data);
                    Error += Decimal(Data);
                    break;
                case 7: // EcoCash Reference
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
            }

            return string.IsNullOrEmpty(Error);
        }

        public override void ExtractRow(List<string> Row)
        {
            var ecoCashReference = Row[6];

            var booking = Bookings.FirstOrDefault(o => o.Id == Convert.ToInt32(Row[1]));
            if (booking != null && !string.IsNullOrEmpty(ecoCashReference))
            {
                booking.PaidOut = true;
                booking.Save();

                new Journal
                {
                    Amount = Convert.ToDecimal(Row[4]),
                    Booking = booking,
                    EcoCashReference = ecoCashReference,
                    Reconciled = false,
                    Title = $"Pay out to {Row[2]} {Row[3]}",
                    TypeId = JournalType.Settlement,
                    UserId = booking.Listing.UserId,
                    Date = DateTime.UtcNow,
                    Region = Region
                }.Save();
            }
        }
    }
}
