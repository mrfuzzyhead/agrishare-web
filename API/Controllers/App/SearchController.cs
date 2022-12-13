using Agrishare.Core.Entities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles = "User")]
    public class SearchController : BaseApiController
    {
        [Route("search")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Sort, int CategoryId, decimal Latitude, decimal Longitude,
            DateTime StartDate, int ServiceId = 0, decimal Size = 0, bool IncludeFuel = true, bool Mobile = false, BookingFor For = BookingFor.Me,
            decimal DestinationLatitude = 0, decimal DestinationLongitude = 0, decimal TotalVolume = 0, string Keywords = "", bool HideUnavailable = false,
            decimal DistanceToWaterSource = 0, decimal DepthOfWaterSource = 0, int LabourServices = 0, int LandRegion = 0, bool ShowVerified = false)
        {
            Counter.Hit(UserId: CurrentUser.Id, Event: Counters.Search, CategoryId: CategoryId);

            if (ServiceId == 0)
                ServiceId = CategoryId;

            var list = ListingSearchResult.List(PageIndex: PageIndex, PageSize: PageSize, Sort: Sort, CategoryId: CategoryId,
                ServiceId: ServiceId, Latitude: Latitude, Longitude: Longitude, StartDate: StartDate, Size: Size, IncludeFuel: IncludeFuel, 
                Mobile: Mobile, For: For, DestinationLatitude: DestinationLatitude, DestinationLongitude: DestinationLongitude, 
                TotalVolume: TotalVolume, Keywords: Keywords, RegionId: CurrentRegion.Id, HideUnavailable: HideUnavailable,
                DistanceToWaterSource: DistanceToWaterSource, DepthOfWaterSource: DepthOfWaterSource, LabourServices: LabourServices,
                LandRegion: LandRegion, ShowVerified: ShowVerified);

            if (list.Count() > 0)
                Counter.Hit(UserId: CurrentUser.Id, Event: Counters.Match, CategoryId: CategoryId);

            var adverts = Advert.List(Live: true, PageSize: 5, Sort: "Random");
            Advert.AddImpressions(adverts.Select(e => e.Id).ToList());

            return Success(new
            {
                List = list.Select(e => e.Json()),
                Adverts = adverts.Select(e => e.AppJson())
            });
        }

        //[Route("result/pdf")]
        //[AcceptVerbs("POST")]
        //public HttpResponseMessage ResultPDF(ListingSearchResult Result)
        //{
        //    var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
        //    var dataStream = new MemoryStream(Result.ListingPDF());
        //    httpResponseMessage.Content = new StreamContent(dataStream);
        //    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = $"{Result.Title}.pdf"
        //    };
        //    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

        //    return httpResponseMessage;
        //}

        [Route("search/result/pdf")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage ResultPDF(int ListingId, DateTime StartDate, DateTime EndDate, decimal Days, decimal TransportDistance, decimal Size, decimal HireCost)
        {
            var listing = Listing.Find(ListingId);
            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            var bytes = ListingSearchResult.ListingPDF(Listing: listing, StartDate: StartDate, EndDate: EndDate, Days: Days, TransportDistance: TransportDistance, Size: Size, HireCost: HireCost, Region: CurrentRegion);
            var dataStream = new MemoryStream(bytes);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = $"{listing.Title}.pdf"
            };
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            return httpResponseMessage;
        }
    }
}
