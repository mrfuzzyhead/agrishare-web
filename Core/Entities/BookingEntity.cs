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
    public partial class Booking : IEntity
    {
        public static string DefaultSort = "StartDate DESC";
        public string Title => Id.ToString() ?? "00000";
        public string For => $"{ForId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public string PaymentStatus => $"{PaymentStatusId}".ExplodeCamelCase();
        public decimal AgriShareCommission => Math.Round(Price - (Price / (1 + Commission)));

        private List<Tag> _tags;
        public List<Tag> Tags
        {
            get
            {
                if (_tags == null && !string.IsNullOrEmpty(TagsJson))
                    _tags = JsonConvert.DeserializeObject<List<Tag>>(TagsJson);
                if (_tags == null)
                    _tags = new List<Tag>();
                return _tags;
            }
            set
            {
                _tags = value;
            }
        }

        private File receiptPhoto;
        public File ReceiptPhoto
        {
            get
            {
                if (receiptPhoto == null && !string.IsNullOrEmpty(ReceiptPhotoPath))
                    receiptPhoto = new File(ReceiptPhotoPath);
                return receiptPhoto;
            }
            set
            {
                receiptPhoto = value;
            }
        }

        public static Booking Find(int Id = 0)
        {
            if (Id == 0)
                return new Booking
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings
                    .Include(o => o.User)
                    .Include(o => o.Service)
                    .Include(o => o.Listing.Region)
                    .Include(o => o.Voucher)
                    .Where(o => !o.Deleted);


                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Booking> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int ListingId = 0, int UserId = 0, int AgentId = 0,
            int SupplierId = 0, DateTime? StartDate = null, DateTime? EndDate = null, BookingStatus Status = BookingStatus.None, bool Upcoming = false, int CategoryId = 0,
            bool? PaidOut = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings
                    .Include(o => o.User.Agent)
                    .Include(o => o.Service)
                    .Include(o => o.Listing.Region)
                    .Include(o => o.Voucher)
                    .Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (RegionId > 0)
                    query = query.Where(o => o.Listing.RegionId == RegionId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (AgentId > 0)
                    query = query.Where(o => o.User.AgentId == AgentId);

                if (SupplierId > 0)
                    query = query.Where(o => o.Listing.UserId == SupplierId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.EndDate >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate <= endDate);
                }

                if (Status != BookingStatus.None)
                    query = query.Where(e => e.StatusId == Status);

                if (Upcoming)
                    query = query.Where(e => e.StatusId == BookingStatus.Approved || e.StatusId == BookingStatus.InProgress);

                if (CategoryId > 0)
                    query = query.Where(e => e.Listing.CategoryId == CategoryId);

                if (PaidOut.HasValue)
                    query = query.Where(e => e.PaidOut == PaidOut.Value);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int ListingId = 0, int UserId = 0, int AgentId = 0, int SupplierId = 0, DateTime? StartDate = null, DateTime? EndDate = null, 
            BookingStatus Status = BookingStatus.None, bool Upcoming = false, int CategoryId = 0, bool? PaidOut = null, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing).Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (RegionId > 0)
                    query = query.Where(o => o.Listing.RegionId == RegionId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (AgentId > 0)
                    query = query.Where(o => o.User.AgentId == AgentId);

                if (SupplierId > 0)
                    query = query.Where(o => o.Listing.UserId == SupplierId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.EndDate >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate <= endDate);
                }

                if (Status != BookingStatus.None)
                    query = query.Where(e => e.StatusId == Status);

                if (Upcoming)
                    query = query.Where(e => e.StatusId == BookingStatus.Approved || e.StatusId == BookingStatus.InProgress);

                if (CategoryId > 0)
                    query = query.Where(e => e.Listing.CategoryId == CategoryId);

                if (PaidOut.HasValue)
                    query = query.Where(e => e.PaidOut == PaidOut.Value);

                return query.Count();
            }
        }

        public static decimal SeekingSummary(int UserId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.UserId == UserId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.Price).DefaultIfEmpty(0).Sum();
            }

        }

        public static decimal SeekingSummaryAgentCommission(int UserId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.UserId == UserId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.HireCost * o.Commission * o.AgentCommission).DefaultIfEmpty(0).Sum();
            }

        }

        public static int SeekingSummaryAgentAdminCount(int AgentId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.User.AgentId == AgentId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Count();
            }

        }

        public static decimal SeekingSummaryAgentAdminCommission(int AgentId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.User.AgentId == AgentId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.HireCost * o.Commission * o.AgentCommission).DefaultIfEmpty(0).Sum();
            }

        }

        public static decimal OfferingSummary(int UserId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.Listing.UserId == UserId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.Price).DefaultIfEmpty(0).Sum();
            }

        }

        public static List<Booking> SettlementReport(DateTime StartDate, DateTime EndDate, int RegionId)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings
                    .Include(o => o.Voucher)
                    .Include(o => o.User.Agent)
                    .Include(o => o.Listing.User)
                    .Where(o => !o.Deleted);

                query = query.Where(o => o.StatusId == BookingStatus.Complete && !o.PaidOut);
                query = query.Where(o => o.Listing.RegionId == RegionId);

                var startDate = StartDate.StartOfDay();
                var endDate = EndDate.EndOfDay();
                query = query.Where(o => o.EndDate >= startDate.Date && o.EndDate <= endDate.Date);

                query = query.OrderBy(o => o.DateCreated);

                var results = query.ToList();
                return results;
            }
        }

        public bool Save()
        {
            var success = false;

            TagsJson = JsonConvert.SerializeObject(Tags.Select(e => e.Json()));

            if (ReceiptPhoto != null)
                ReceiptPhotoPath = ReceiptPhoto.Filename;

            var service = Service;
            if (service != null)
                ServiceId = service.Id;
            Service = null;

            var listing = Listing;
            if (listing != null)
                ListingId = listing.Id;
            Listing = null;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Service = service;
            Listing = listing;
            User = user;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Bookings.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Bookings.Attach(this);
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
                ForId,
                For,
                User = User?.Json(),
                Listing = Listing?.Json(),
                Service = Service?.Json(),
                Voucher = Voucher?.Json(),
                Location,
                Latitude,
                Longitude,
                Destination,
                DestinationLatitude,
                DestinationLongitude,
                Quantity,
                Distance,
                IncludeFuel,
                StartDate,
                EndDate,
                Price,
                AgriShareCommission,
                Commission,
                AgentCommission,
                TransactionFee,
                IMTT,
                SMSCost,
                SMSCount,
                HireCost,
                FuelCost,
                VoucherTotal,
                TransportCost,
                TransportDistance,
                AdditionalInformation,
                TotalVolume,
                StatusId,
                Status,
                PaymentStatusId,
                PaymentStatus,
                PaidOut,
                Tags = Tags.Select(e => e.Json()),
                DateCreated,
                ReceiptPhoto = ReceiptPhoto?.JSON(),
                PaymentMethodId,
                PaymentMethod = $"{PaymentMethodId}".ExplodeCamelCase()
            };
        }

        public static decimal TotalAmountPaid(DateTime StartDate, DateTime EndDate, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();
                var query = ctx.Bookings.Where(o => !o.Deleted && o.StatusId == BookingStatus.Complete && o.StartDate <= EndDate && o.EndDate >= StartDate);
                if (RegionId > 0)
                    query = query.Where(e => e.Listing.RegionId == RegionId);
                return query.Select(e => e.Price).DefaultIfEmpty(0).Sum();
            }
        }

        public static decimal TotalAgriShareCommission(DateTime StartDate, DateTime EndDate, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();
                var query = ctx.Bookings.Where(o => !o.Deleted && o.StatusId == BookingStatus.Complete && o.StartDate <= EndDate && o.EndDate >= StartDate);
                if (RegionId > 0)
                    query = query.Where(e => e.Listing.RegionId == RegionId);
                return query.Select(e => e.Price * (e.Commission - (e.Commission * e.AgentCommission))).DefaultIfEmpty(0).Sum();
            }
        }

        public static decimal TotalAgentsCommission(DateTime StartDate, DateTime EndDate, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();
                var query = ctx.Bookings.Where(o => !o.Deleted && o.Listing.RegionId == RegionId && o.StatusId == BookingStatus.Complete && o.StartDate <= EndDate && o.EndDate >= StartDate);
                if (RegionId > 0)
                    query = query.Where(e => e.Listing.RegionId == RegionId);
                return query.Select(e => e.Price * (e.Commission * e.AgentCommission)).DefaultIfEmpty(0).Sum();
            }
        }

        public static decimal TotalFeesIncurred(DateTime StartDate, DateTime EndDate, int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();
                var query = ctx.Bookings.Where(o => !o.Deleted && o.Listing.RegionId == RegionId && o.StatusId == BookingStatus.Complete && o.StartDate <= EndDate && o.EndDate >= StartDate);
                if (RegionId > 0)
                    query = query.Where(e => e.Listing.RegionId == RegionId);
                return query.Select(e => e.SMSCount + e.TransactionFee + e.IMTT).DefaultIfEmpty(0).Sum();
            }
        }

        public static List<BookingCounter> Summary(int RegionId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = $"SELECT Bookings.StatusId AS `Status`, Bookings.ForId AS `For`, COUNT(Bookings.Id) AS `Count` FROM Bookings ";
                if (RegionId > 0)
                    sql += $"INNER JOIN Listings ON Bookings.ListingId = Listings.Id WHERE Listings.RegionId = {RegionId} ";
                sql += "GROUP BY Bookings.StatusId, Bookings.ForId";
                return ctx.Database.SqlQuery<BookingCounter>(sql).ToList();
            }
        }

        public static List<GraphData> Graph(DateTime StartDate, DateTime EndDate, int RegionId = 0, int UserId = 0, BookingStatus Status = BookingStatus.None, int CategoryId = 0, int Count = 6, int AgentId = 0)
        {
            var sql = $@"SELECT MONTH(Bookings.StartDate) AS `Month`, YEAR(Bookings.StartDate) AS `Year`, COUNT(Bookings.Id) AS 'Count' 
                            FROM Bookings
                            INNER JOIN Listings ON Bookings.ListingId = Listings.Id
                            INNER JOIN Users ON Listings.UserId = Users.Id
                            WHERE 
                                DATE(Bookings.StartDate) <= DATE('{EndDate.ToString("yyyy-MM-dd")}') AND 
                                DATE(Bookings.EndDate) >= DATE('{StartDate.ToString("yyyy-MM-dd")}') ";

            if (UserId > 0)
                sql += $"AND Bookings.UserId = {UserId} ";

            if (RegionId > 0)
                sql += $"AND Listings.RegionId = {RegionId} ";

            if (AgentId > 0)
                sql += $"AND Users.AgentId = {AgentId} ";

            if (CategoryId > 0)
                sql += $"AND Listings.CategoryId = {CategoryId} ";

            if (Status != BookingStatus.None)
                sql += $"AND Bookings.StatusId = {(int)Status} ";

            sql += $@"GROUP BY MONTH(Bookings.StartDate), YEAR(Bookings.StartDate) ORDER BY YEAR(Bookings.StartDate), MONTH(Bookings.StartDate) LIMIT {Count}";

            using (var ctx = new AgrishareEntities())
                return ctx.Database.SqlQuery<GraphData>(sql).ToList();
        }

        public class GraphData
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public int Count { get; set; }
        }

        public byte[] InvoicePDF()
        {
            var content = Template.Find(Title: "Invoice PDF");
            content.Replace("Invoice Number", $"INV-{Id.ToString().PadLeft(8, '0')}");
            content.Replace("Recipient Name", User.Title);
            content.Replace("Recipient Telephone", User.Telephone);
            content.Replace("Invoice Date", DateCreated.ToString("d MMMM yyyy"));
            content.Replace("Service Title", Listing.Title);
            content.Replace("Hire Cost", Price.ToString("N2"));
            return PDF.ConvertHtmlToPdf(content.HTML, Config.WebURL);
        }

        public bool PopReceived()
        {
            if (StatusId == BookingStatus.Approved)
                StatusId = BookingStatus.Paid;

            if (Save())
            {
                var template = Template.Find(Title: "Booking Paid");
                template.Replace("User", User.Title);
                template.Replace("Booking", $"Booking #{Id}");
                template.Replace("Booking URL", $"{Config.CMSURL}/#/bookings/detail/{Id}");

                new Email
                {
                    Message = template.EmailHtml(),
                    RecipientEmail = Config.ApplicationEmailAddress,
                    SenderEmail = Config.ApplicationEmailAddress,
                    Subject = $"Booking Payment #{Id}"
                }.Send();

                return true;
            }

            return false;
        }
    }

    public class BookingCounter
    {
        public BookingStatus Status { get; set; }
        public BookingFor For { get; set; }
        public int Count { get; set; }
    }
}
