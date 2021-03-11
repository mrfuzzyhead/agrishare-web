using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public enum ListingSearchResultSort
    {
        Distance,
        Price
    }

    public class ListingSearchResult
    {
        public int ListingId { get; set; }
        public int ServiceId { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public ListingCondition ConditionId { get; set; }
        public string Condition => $"{ConditionId}".ExplodeCamelCase();
        public double AverageRating { get; set; }
        public string PhotoPaths { get; set; }
        public double Distance { get; set; }
        public bool Available { get; set; }
        public double Price { get; set; }
        public double TotalVolume { get; set; }
        public double Size { get; set; }
        public double TransportDistance { get; set; }
        public double TransportCost { get; set; }
        public double FuelCost { get; set; }
        public double HireCost { get; set; }
        public List<File> Photos => PhotoPaths?.Split(',').Select(e => new File(e)).ToList();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }
        public int Trips { get; set; }

        public static int Count(int CategoryId, int ServiceId, decimal Latitude,
            decimal Longitude, DateTime StartDate, int Size, bool IncludeFuel, bool Mobile, BookingFor For, decimal DestinationLatitude,
            decimal DestinationLongitude, decimal TotalVolume, int ListingId = 0, string Keywords = "", int RegionId = 0, 
            bool HideUnavailable = false, decimal DistanceToWaterSource = 0, decimal DepthOfWaterSource = 0, int LabourServices = 0, int LandRegion = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT");
                sql.AppendLine("Services.Id AS ServiceId,");
                sql.AppendLine("Listings.Id AS ListingId,");
                sql.AppendLine("Listings.Title AS Title,");
                sql.AppendLine("Listings.Year AS YEAR,");
                sql.AppendLine("Listings.ConditionId AS ConditionId,");
                sql.AppendLine("Listings.AverageRating AS AverageRating,");
                sql.AppendLine("Listings.Photos AS PhotoPaths,");

                // distances
                var distance = SQL.Distance(Latitude, Longitude, "Listings");
                var depotToPickup = SQL.Distance(Latitude, Longitude, "Listings");
                var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                var dropoffToDepot = SQL.Distance(DestinationLatitude, DestinationLongitude, "Listings");
                var trips = $"0";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    trips = $"(CEIL({TotalVolume} / Services.TotalVolume))";

                var transportDistance = $"0";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    transportDistance = $"({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * ({trips} - 1)))";
                else if (CategoryId == Category.BusId)
                    transportDistance = $"({depotToPickup} + {dropoffToDepot} + {pickupToDropoff})";
                else if (CategoryId == Category.LandId)
                    transportDistance = $"(-1)";
                else if (Mobile)
                    transportDistance = $"{distance} * 2";

                // size
                string sizeField = "";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                {
                    sizeField = $"({trips})";
                    sql.AppendLine($"{sizeField} AS Size,");
                }
                else if (CategoryId == Category.BusId)
                {
                    sizeField = $"1";
                    sql.AppendLine($"{sizeField} AS Size,");
                }
                else
                {
                    sizeField = $"({Size})";
                    sql.AppendLine($"{sizeField} AS Size,");
                }

                // time
                var days = $"CEIL((Services.TimePerQuantityUnit * {sizeField}) / 8)";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                {
                    var totalDistance = $"(({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * (({trips} * 2) - 1))) / 100)";
                    days = $"CEIL((Services.TimePerQuantityUnit * {totalDistance}) / 8)";
                }
                else if (CategoryId == Category.BusId)
                {
                    var totalDistance = $"(({depotToPickup} + {dropoffToDepot} + {pickupToDropoff}) / 100)";
                    days = $"CEIL((Services.TimePerQuantityUnit * {totalDistance}) / 8)";
                }
                else if (CategoryId == Category.ProcessingId)
                {
                    days = $"CEIL(({sizeField} / Services.TimePerQuantityUnit) / 8)";
                }

                // availability
                sql.AppendLine($"IF((SELECT COUNT(Id) FROM Bookings WHERE Bookings.Deleted = 0 AND Bookings.StatusId IN (0, 1, 3, 6) AND Bookings.ListingId = Listings.Id AND Bookings.StartDate <= DATE_ADD(DATE('{SQL.Safe(StartDate)}'), INTERVAL {days} DAY) AND Bookings.EndDate >= DATE('{SQL.Safe(StartDate)}')) = 0, 1, 0) AS Available,");

                // costs
                var hireCost = $"(Services.PricePerQuantityUnit * {Size} * {1 + Transaction.AgriShareCommission})";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    hireCost = $"(Services.PricePerQuantityUnit * {trips} * {1 + Transaction.AgriShareCommission})";
                var fuelCost = $"0";
                if ((CategoryId != Category.LorriesId || ServiceId == Category.TractorTransportServiceId) && IncludeFuel)
                    fuelCost = $"(Services.FuelPerQuantityUnit * {Size} * Services.FuelPrice * {1 + Transaction.AgriShareCommission})";
                var transportCost = $"({transportDistance} * Services.PricePerDistanceUnit * {1 + Transaction.AgriShareCommission})";

                sql.AppendLine($"ROUND({transportCost}) AS TransportCost,");
                sql.AppendLine($"ROUND({fuelCost}) AS FuelCost,");
                sql.AppendLine($"ROUND({hireCost}) AS HireCost,");
                sql.AppendLine($"ROUND({hireCost}) + ROUND({fuelCost}) + ROUND({transportCost}) AS Price,");
                sql.AppendLine($"{transportDistance} AS TransportDistance,");
                sql.AppendLine($"{distance} AS Distance,");
                sql.AppendLine($"{trips} AS Trips,");
                sql.AppendLine($"{days} AS Days,");

                sql.AppendLine($"DATE('{SQL.Safe(StartDate)}') AS StartDate,");
                sql.AppendLine($"DATE_ADD('{SQL.Safe(StartDate)}', INTERVAL ({days} - 1) DAY) AS EndDate");
                sql.AppendLine("FROM Listings");
                sql.AppendLine("INNER JOIN Services ON Listings.Id = Services.ListingId");
                sql.AppendLine("WHERE Listings.Deleted = 0 AND Services.Deleted = 0 AND Listings.StatusId = 1");
                sql.AppendLine($"AND Listings.RegionId = {RegionId}");
                sql.AppendLine($"AND Listings.CategoryId = {CategoryId}");
                sql.AppendLine($"AND Services.Mobile = {SQL.Safe(Mobile)}");
                sql.AppendLine($"AND Services.CategoryId = {ServiceId}");

                if (HideUnavailable)
                    sql.AppendLine($"AND Available = 1");

                //BS: 2020-02-26 removed limitation checks
                //sql.AppendLine($"AND {Size} >= MinimumQuantity");
                //if (Mobile)
                //    sql.AppendLine($"AND {distance} <= Services.MaximumDistance");

                if (DistanceToWaterSource > 0)
                    sql.AppendLine($"AND {DistanceToWaterSource} <= Services.MaximumDistanceToWaterSource");

                if (DepthOfWaterSource > 0)
                    sql.AppendLine($"AND {DepthOfWaterSource} <= Services.MaximumDepthOfWaterSource");

                if (CategoryId == Category.LandId)
                    sql.AppendLine($"AND {TotalVolume} <= Services.AvailableAcres AND {TotalVolume} >= Services.MinimumAcres");

                if (For == BookingFor.Group)
                    sql.AppendLine($"AND Listings.GroupServices = 1");

                if (IncludeFuel)
                    sql.AppendLine($"AND Listings.AvailableWithFuel = 1");
                else
                    sql.AppendLine($"AND Listings.AvailableWithoutFuel = 1");

                if (LabourServices > 0)
                    sql.AppendLine($"AND (Services.LabourServices & {LabourServices}) = {LabourServices}");

                if (LandRegion > 0)
                    sql.AppendLine($"AND Services.LandRegion = {LandRegion}");

                if (ListingId > 0)
                    sql.AppendLine($"AND Listings.Id = {ListingId}");

                if (!string.IsNullOrEmpty(Keywords))
                    sql.AppendLine($@"AND Listings.Title LIKE '%{Keywords.SqlSafe()}%'");

                return ctx.Database.SqlQuery<ListingSearchResult>(sql.ToString()).Count();
            }
        }

        public static List<ListingSearchResult> List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, decimal Latitude,
            decimal Longitude, DateTime StartDate, decimal Size, bool IncludeFuel, bool Mobile, BookingFor For, decimal DestinationLatitude,
            decimal DestinationLongitude, decimal TotalVolume, int ListingId = 0, string Keywords = "", int RegionId = 0, 
            bool HideUnavailable = false, decimal DistanceToWaterSource = 0, decimal DepthOfWaterSource = 0, int LabourServices = 0, int LandRegion = 0)
        {
            var sort = ListingSearchResultSort.Distance;
            try { sort = (ListingSearchResultSort)Enum.Parse(typeof(ListingSearchResultSort), Sort); }
            catch { }

            using (var ctx = new AgrishareEntities())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT");
                sql.AppendLine("Services.Id AS ServiceId,");
                sql.AppendLine("Listings.Id AS ListingId,");
                sql.AppendLine("Listings.Title AS Title,");
                sql.AppendLine("Listings.Year AS YEAR,");
                sql.AppendLine("Listings.ConditionId AS ConditionId,");
                sql.AppendLine("Listings.AverageRating AS AverageRating,");
                sql.AppendLine("Listings.Photos AS PhotoPaths,");
                sql.AppendLine($"{TotalVolume} AS TotalVolume,");

                // distances
                var distance = SQL.Distance(Latitude, Longitude, "Listings");
                if (CategoryId == Category.LandId)
                    distance = $"(-1)";
                var depotToPickup = SQL.Distance(Latitude, Longitude, "Listings");
                var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                var dropoffToDepot = SQL.Distance(DestinationLatitude, DestinationLongitude, "Listings");
                var trips = $"0";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    trips = $"(CEIL({TotalVolume} / Services.TotalVolume))";

                var transportDistance = $"0";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    transportDistance = $"({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * ({trips} - 1)))";
                else if (CategoryId == Category.BusId)
                    transportDistance = $"({depotToPickup} + {dropoffToDepot} + {pickupToDropoff})";
                else if (CategoryId == Category.LandId)
                    transportDistance = $"(-1)";
                else if (Mobile)
                    transportDistance = $"{distance} * 2";

                // size
                string sizeField = "";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                {
                    sizeField = $"({trips})";
                }
                else if (CategoryId == Category.BusId)
                {
                    sizeField = $"1";
                }
                else
                {
                    sizeField = $"({Size})";
                }
                sql.AppendLine($"{sizeField} AS Size,");

                // time
                var days = $"CEIL((Services.TimePerQuantityUnit * {sizeField}) / 8)";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                {
                    var totalDistance = $"(({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * (({trips} * 2) - 1))) / 100)";
                    days = $"CEIL((Services.TimePerQuantityUnit * {totalDistance}) / 8)";
                }
                else if (CategoryId == Category.BusId)
                {
                    var totalDistance = $"(({depotToPickup} + {dropoffToDepot} + {pickupToDropoff}) / 100)";
                    days = $"CEIL((Services.TimePerQuantityUnit * {totalDistance}) / 8)";
                }
                else if (CategoryId == Category.IrrigationId || CategoryId == Category.LabourId)
                {                    
                    days = $"({sizeField})";
                }
                else if (CategoryId == Category.ProcessingId)
                {
                    days = $"CEIL(({sizeField} / Services.TimePerQuantityUnit) / 8)";
                }

                // availability
                sql.AppendLine($"IF((SELECT COUNT(Id) FROM Bookings WHERE Bookings.Deleted = 0 AND Bookings.StatusId IN (0, 1, 3, 6) AND Bookings.ListingId = Listings.Id AND Bookings.StartDate <= DATE_ADD(DATE('{SQL.Safe(StartDate)}'), INTERVAL {days} DAY) AND Bookings.EndDate >= DATE('{SQL.Safe(StartDate)}')) = 0, 1, 0) AS Available,");

                // costs
                var hireCost = $"(Services.PricePerQuantityUnit * {Size} * {1 + Transaction.AgriShareCommission})";
                if (CategoryId == Category.LorriesId || ServiceId == Category.TractorTransportServiceId)
                    hireCost = $"(Services.PricePerQuantityUnit * {trips} * {1 + Transaction.AgriShareCommission})";
                var fuelCost = $"0";
                if ((CategoryId != Category.LorriesId || ServiceId == Category.TractorTransportServiceId) && IncludeFuel)
                    fuelCost = $"(Services.FuelPerQuantityUnit * {Size} * Services.FuelPrice * {1 + Transaction.AgriShareCommission})";
                var transportCost = $"({transportDistance} * Services.PricePerDistanceUnit * {1 + Transaction.AgriShareCommission})";

                sql.AppendLine($"ROUND({transportCost}) AS TransportCost,");
                sql.AppendLine($"ROUND({fuelCost}) AS FuelCost,");
                sql.AppendLine($"ROUND({hireCost}) AS HireCost,");
                sql.AppendLine($"ROUND({hireCost}) + ROUND({fuelCost}) + ROUND({transportCost}) AS Price,");
                sql.AppendLine($"{transportDistance} AS TransportDistance,");
                sql.AppendLine($"{distance} AS Distance,");
                sql.AppendLine($"{trips} AS Trips,");
                sql.AppendLine($"{days} AS Days,");

                sql.AppendLine($"DATE('{SQL.Safe(StartDate)}') AS StartDate,");
                sql.AppendLine($"DATE_ADD('{SQL.Safe(StartDate)}', INTERVAL ({days} - 1) DAY) AS EndDate");
                sql.AppendLine("FROM Listings");
                sql.AppendLine("INNER JOIN Services ON Listings.Id = Services.ListingId");
                sql.AppendLine("WHERE Listings.Deleted = 0 AND Services.Deleted = 0 AND Listings.StatusId = 1");
                sql.AppendLine($"AND Listings.RegionId = {RegionId}");
                sql.AppendLine($"AND Listings.CategoryId = {CategoryId}");
                sql.AppendLine($"AND Services.Mobile = {SQL.Safe(Mobile)}");
                sql.AppendLine($"AND Services.CategoryId = {ServiceId}");

                if (HideUnavailable)
                    sql.AppendLine($"AND Available = 1");

                //BS: 2020-02-26 removed limitation checks
                //sql.AppendLine($"AND {Size} >= MinimumQuantity");
                //if (Mobile)
                //    sql.AppendLine($"AND {distance} <= Services.MaximumDistance");

                if (DistanceToWaterSource > 0)
                    sql.AppendLine($"AND {DistanceToWaterSource} <= Services.MaximumDistanceToWaterSource");

                if (DepthOfWaterSource > 0)
                    sql.AppendLine($"AND {DepthOfWaterSource} <= Services.MaximumDepthOfWaterSource");

                if (CategoryId == Category.LandId)
                    sql.AppendLine($"AND {TotalVolume} <= Services.AvailableAcres AND {TotalVolume} >= Services.MinimumAcres");

                if (LabourServices > 0)
                    sql.AppendLine($"AND (Services.LabourServices & {LabourServices}) = {LabourServices}");

                if (LandRegion > 0)
                    sql.AppendLine($"AND Services.LandRegion = {LandRegion}");


                if (For == BookingFor.Group)
                    sql.AppendLine($"AND Listings.GroupServices = 1");

                if (IncludeFuel)
                    sql.AppendLine($"AND Listings.AvailableWithFuel = 1");
                else
                    sql.AppendLine($"AND Listings.AvailableWithoutFuel = 1");

                if (ListingId > 0)
                    sql.AppendLine($"AND Listings.Id = {ListingId}");

                if (!string.IsNullOrEmpty(Keywords))
                    sql.AppendLine($@"AND Listings.Title LIKE '%{Keywords.SqlSafe()}%'");

                sql.AppendLine($"GROUP BY Services.Id ORDER BY {sort} LIMIT {PageIndex * PageSize}, {PageSize}");

                #if DEBUG
                Log.Debug("Search SQL", sql.ToString());
                #endif

                return ctx.Database.SqlQuery<ListingSearchResult>(sql.ToString()).ToList();
            }
        }

        public object Json()
        {
            return new
            {
                ServiceId,
                ListingId,
                Title,
                Year,
                Condition,
                AverageRating,
                Photos = Photos?.Select(e => e.JSON()),
                Distance,
                Price,
                TotalVolume,
                Size,
                Trips,
                TransportDistance,
                TransportCost,
                FuelCost,
                HireCost,
                Available,
                StartDate,
                EndDate,
                Days
            };
        }

        public object BookingJson()
        {
            return new
            {
                ServiceId,
                ListingId,
                Distance,
                Price,
                TotalVolume,
                Size,
                Trips,
                TransportDistance,
                TransportCost,
                FuelCost,
                HireCost,
                Available,
                StartDate,
                EndDate,
                Days
            };
        }

        public byte[] ListingPDF ()
        {
            var listing = Listing.Find(ListingId);

            var content = Template.Find(Title: "Listing PDF");
            content.Replace("Listing ID", ListingId.ToString());

            if (Photos.Count > 0)
                content.Replace("Listing Photo", $"{Config.CDNURL}/{Photos.First().ThumbName}");
            else
                content.Replace("Listing Photo", string.Empty);
            content.Replace("Listing Title", Title);
            content.Replace("Listing Description", listing.Description);

            if (Days <= 1)
                content.Replace("Date", StartDate.ToString("d MMMM yyyy"));
            else
                content.Replace("Date", StartDate.ToString("d MMMM yyyy") + " - " + EndDate.ToString("d MMMM yyyy"));

            if (Days > 1)
                content.Replace("Duration", $"{Days} days");
            else
                content.Replace("Duration", $"1 day");

            if (TransportDistance > 0)
                content.Replace("Distance", $"{TransportDistance}km");
            else
                content.Replace("Distance", $"-");

            if (listing.CategoryId == Category.LorriesId)
                content.Replace("Size", $"{TotalVolume} tonnes");
            else if (listing.CategoryId == Category.ProcessingId)
                content.Replace("Size", $"{Size} kgs");
            else if (listing.CategoryId == Category.TractorsId)
                content.Replace("Size", $"{Size} ha");
            if (listing.CategoryId == Category.BusId)
                content.Replace("Size", $"-");

            content.Replace("Total Cost", HireCost.ToString("N2"));

            return PDF.ConvertHtmlToPdf(content.HTML, Config.WebURL);
        }

        public static byte[] ListingPDF(Listing Listing, DateTime StartDate, DateTime EndDate, decimal Days, decimal TransportDistance, decimal Size, decimal HireCost, Region Region)
        {
            var content = Template.Find(Title: "Listing PDF");
            content.Replace("Listing ID", Listing.Id.ToString());

            if (Listing.Photos.Count > 0)
                content.Replace("Listing Photo", $"{Config.CDNURL}/{Listing.Photos.First().ThumbName}");
            else
                content.Replace("Listing Photo", string.Empty);
            content.Replace("Listing Title", Listing.Title);
            content.Replace("Listing Description", Listing.Description);

            if (Days <= 1)
                content.Replace("Date", StartDate.ToString("d MMMM yyyy"));
            else
                content.Replace("Date", StartDate.ToString("d MMMM yyyy") + " - " + EndDate.ToString("d MMMM yyyy"));

            if (Days > 1)
                content.Replace("Duration", $"{Days.ToString("N0")} days");
            else
                content.Replace("Duration", $"1 day");

            if (TransportDistance > 0)
                content.Replace("Distance", $"{TransportDistance.ToString("N0")}km");
            else
                content.Replace("Distance", $"-");

            if (Listing.CategoryId == Category.LorriesId)
                content.Replace("Size", $"{Size.ToString("N2")} tonnes");
            else if (Listing.CategoryId == Category.ProcessingId)
                content.Replace("Size", $"{Size.ToString("N2")} kgs");
            else if (Listing.CategoryId == Category.TractorsId)
                content.Replace("Size", $"{Size.ToString("N2")} ha");
            if (Listing.CategoryId == Category.BusId)
                content.Replace("Size", $"-");

            content.Replace("Total Cost", $"{Region.Currency}{HireCost.ToString("N2")}");

            return PDF.ConvertHtmlToPdf(content.HTML, Config.WebURL);
        }
    }
}
