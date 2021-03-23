using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class ListingsController : BaseApiController
    {
        [Route("listings")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, int CategoryId = 0)
        {
            var list = Entities.Listing.List(UserId: CurrentUser.Id, PageIndex: PageIndex, PageSize: PageSize, CategoryId: CategoryId);
            return Success(new
            {
                List = list.Select(e => e.Json())
            });
        }

        [Route("listings/detail")]
        [AcceptVerbs("GET")]
        public object Detail(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);

            if (listing == null || listing.Id == 0)
                return Error("Listing not found");

            var upcomingBookings = Entities.Booking.Count(UserId: CurrentUser.Id, StartDate: DateTime.Now);

            return Success(new
            {
                Listing = listing.Json(),
                UpcomingBookings = upcomingBookings
            });
        }

        [Route("listings/add")]
        [AcceptVerbs("POST")]
        public object Add(ListingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (!Model.Latitude.HasValue || !Model.Longitude.HasValue)
                return Error("Location is required");

            var listing = new Listing
            {
                AvailableWithoutFuel = Model.AvailableWithoutFuel,
                AvailableWithFuel = Model.AvailableWithFuel,
                Brand = Model.Brand,
                CategoryId = Model.CategoryId,
                ConditionId = Model.ConditionId,
                Latitude = Model.Latitude ?? 0,
                Longitude = Model.Longitude ?? 0,
                Description = Model.Description,
                GroupServices = Model.GroupServices,
                HorsePower = Model.HorsePower,
                Location = Model.Location,
                StatusId = Entities.ListingStatus.Live,
                Title = Model.Title,
                UserId = CurrentUser.Id,
                Year = Model.Year,
                Region = CurrentUser.Region
            };

            listing.Services = new List<Service>();
            foreach (var service in Model.Services)
                listing.Services.Add(new Service
                {
                    DistanceUnitId = service.DistanceUnitId,
                    FuelPrice = service.FuelPrice,
                    FuelPerQuantityUnit = service.FuelPerQuantityUnit,
                    MaximumDistance = service.MaximumDistance,
                    MinimumQuantity = service.MinimumQuantity,
                    Mobile = service.Mobile || listing.CategoryId == Category.TractorsId || listing.CategoryId == Category.LorriesId || listing.CategoryId == Category.IrrigationId || listing.CategoryId == Category.LabourId,
                    PricePerDistanceUnit = service.Mobile ? service.PricePerDistanceUnit ?? 0 : 0,
                    PricePerQuantityUnit = service.PricePerQuantityUnit,
                    QuantityUnitId = service.QuantityUnitId,
                    CategoryId = service.CategoryId,
                    TimePerQuantityUnit = service.TimePerQuantityUnit,
                    TimeUnitId = service.TimeUnitId,
                    TotalVolume = service.TotalVolume,
                    LabourServices = service.Services,
                    MaximumDistanceToWaterSource = service.MaximumDistanceToWaterSource,
                    MaximumDepthOfWaterSource = service.MaximumDepthOfWaterSource,
                    UnclearedLand = service.UnclearedLand,
                    ClearedLand = service.ClearedLand,
                    NearWaterSource = service.NearWaterSource,
                    FertileSoil = service.FertileSoil,
                    MaxRentalYears = service.MaxRentalYears,
                    AvailableAcres = service.AvailableAcres,
                    MinimumAcres = service.MinimumAcres,
                    LandRegion = service.LandRegion
                });

            if (listing.Services.Count == 0)
                return Error("You must enable at least on service");

            if (listing.CategoryId == Entities.Category.LorriesId && listing.Services.First().TotalVolume == 0)
                return Error("Please enter a valid Total Volume");

            if (listing.CategoryId == Entities.Category.LorriesId || listing.CategoryId == Entities.Category.ProcessingId)
                listing.AvailableWithFuel = listing.AvailableWithoutFuel = true;

            var photos = new List<string>();
            if (Model.Photos != null)
                foreach (var photo in Model.Photos)
                {
                    var filename = Entities.File.SaveBase64Image(photo.Base64);
                    var file = new Entities.File(filename);
                    file.Resize(200, 200, file.ThumbName);
                    file.Resize(800, 800, file.ZoomName);
                    photos.Add(filename);
                }
            if (photos.Count > 0)
                listing.PhotoPaths = string.Join(",", photos.ToArray());

            if (listing.Save())
            {
                listing = Entities.Listing.Find(listing.Id);
                return Success(new
                {
                    Listing = listing.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("listings/edit")]
        [AcceptVerbs("POST")]
        public object Edit(ListingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (!Model.Latitude.HasValue || !Model.Longitude.HasValue)
                return Error("Location is required");

            var listing = Entities.Listing.Find(Id: Model.Id);
            if (listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.AvailableWithoutFuel = Model.AvailableWithoutFuel;
            listing.AvailableWithFuel = Model.AvailableWithFuel;
            listing.Brand = Model.Brand;
            listing.Category = null;
            listing.CategoryId = Model.CategoryId;
            listing.ConditionId = Model.ConditionId;
            listing.Latitude = Model.Latitude ?? 0;
            listing.Longitude = Model.Longitude ?? 0;
            listing.Description = Model.Description;
            listing.GroupServices = Model.GroupServices;
            listing.HorsePower = Model.HorsePower;
            listing.Location = Model.Location;            
            listing.Title = Model.Title;
            listing.Year = Model.Year;

            listing.Services = new List<Service>();
            foreach (var service in Model.Services)
                listing.Services.Add(new Service
                {
                    Id = service.Id,
                    DistanceUnitId = service.DistanceUnitId,
                    FuelPrice = service.FuelPrice,
                    FuelPerQuantityUnit = service.FuelPerQuantityUnit,
                    MaximumDistance = service.MaximumDistance,
                    MinimumQuantity = service.MinimumQuantity,
                    Mobile = service.Mobile || listing.CategoryId == Category.TractorsId || listing.CategoryId == Category.LorriesId || listing.CategoryId == Category.IrrigationId || listing.CategoryId == Category.LabourId,
                    PricePerDistanceUnit = service.Mobile ? service.PricePerDistanceUnit ?? 0 : 0,
                    PricePerQuantityUnit = service.PricePerQuantityUnit,
                    QuantityUnitId = service.QuantityUnitId,
                    CategoryId = service.CategoryId,
                    TimePerQuantityUnit = service.TimePerQuantityUnit,
                    TimeUnitId = service.TimeUnitId,
                    TotalVolume = service.TotalVolume,
                    LabourServices = service.Services,
                    MaximumDistanceToWaterSource = service.MaximumDistanceToWaterSource,
                    MaximumDepthOfWaterSource = service.MaximumDepthOfWaterSource,
                    UnclearedLand = service.UnclearedLand,
                    ClearedLand = service.ClearedLand,
                    NearWaterSource = service.NearWaterSource,
                    FertileSoil = service.FertileSoil,
                    MaxRentalYears = service.MaxRentalYears,
                    AvailableAcres = service.AvailableAcres,
                    MinimumAcres = service.MinimumAcres,
                    LandRegion = service.LandRegion
                });

            var photos = new List<string>();
            if (Model.Photos != null)
                foreach (var photo in Model.Photos)
                {
                    if (photo.Filename.IsEmpty())
                    {
                        var filename = Entities.File.SaveBase64Image(photo.Base64);
                        var file = new Entities.File(filename);
                        file.Resize(200, 200, file.ThumbName);
                        file.Resize(800, 800, file.ZoomName);
                        photos.Add(filename);
                    }
                    else
                        photos.Add(photo.Filename);
                }
            if (photos.Count > 0)
                listing.PhotoPaths = string.Join(",", photos.ToArray());

            if (listing.Save())
            {
                listing = Entities.Listing.Find(listing.Id);
                return Success(new
                {
                    Listing = listing.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("listings/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing?.Id == 0 || listing?.UserId != CurrentUser.Id)
                return Error("Listing not found");

            var bookings = Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
                return Error("Can not delete listing - you have upcoming bookings");

            if (listing.Delete())
                return Success(new
                {
                    listing.Id
                });

            return Error("An unknown error occurred");
        }

        [Route("listings/availability")]
        [AcceptVerbs("GET")]
        public object Availability(int ListingId, DateTime StartDate, DateTime EndDate, int Days = 1, int Volume = 0)
        {
            var listing = Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0)
                return Error("Listing not found");

            var bookings = Booking.List(ListingId: ListingId, StartDate: StartDate, EndDate: StartDate.AddDays(Days));

            var list = new List<object>();

            if (listing.CategoryId == Category.LandId)
            {
                if (listing.CategoryId == Category.LandId)
                    EndDate = StartDate.AddMonths(12);

                var date = StartDate;
                while (date <= EndDate)
                {
                    var start = date.StartOfDay();
                    var end = date.AddDays(Days - 1).EndOfDay();

                    var bookedVolume = bookings.Where(o =>
                        (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.InProgress || o.StatusId == BookingStatus.Pending || o.StatusId == BookingStatus.Incomplete) &&
                        o.StartDate <= end &&
                        o.EndDate >= start).Sum(e => e.TotalVolume);

                    var available = bookedVolume + Volume <= listing.Services.First().AvailableAcres;

                    list.Add(new
                    {
                        Date = date,
                        Available = available
                    });

                    date = date.AddMonths(1);
                }
            }
            else
            {
                var date = StartDate;
                while (date <= EndDate)
                {
                    var start = date.StartOfDay();
                    var end = date.AddDays(Days - 1).EndOfDay();

                    var available = bookings.Where(o =>
                        (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.InProgress || o.StatusId == BookingStatus.Pending || o.StatusId == BookingStatus.Incomplete) &&
                        o.StartDate <= end &&
                        o.EndDate >= start).Count() == 0;

                    list.Add(new
                    {
                        Date = date,
                        Available = available
                    });

                    date = date.AddDays(1);
                }
            }

            return Success(new
            {
                Calendar = list
            });
        }

        [Route("listings/hide")]
        [AcceptVerbs("GET")]
        public object Hide(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.StatusId = Entities.ListingStatus.Hidden;
            if (listing.Save())
                return Success(new
                {
                    Listing = listing.Json()
                });

            return Error("An unknown error occurred");
        }

        [Route("listings/show")]
        [AcceptVerbs("GET")]
        public object Show(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.StatusId = Entities.ListingStatus.Live;
            if (listing.Save())
                return Success(new
                {
                    Listing = listing.Json()
                });

            return Error("An unknown error occurred");
        }
    }
}
