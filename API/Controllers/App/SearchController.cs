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
        public object List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, decimal Latitude, decimal Longitude, 
            DateTime StartDate, decimal Size, bool IncludeFuel, bool Mobile, BookingFor For = BookingFor.Me, 
            decimal DestinationLatitude = 0, decimal DestinationLongitude = 0, decimal TotalVolume = 0, string Keywords = "")
        {
            Counter.Hit(CurrentUser.Id, Counters.Search, ServiceId);

            var list = ListingSearchResult.List(PageIndex, PageSize, Sort, CategoryId, ServiceId, Latitude, Longitude, StartDate, Size, IncludeFuel, Mobile, For, DestinationLatitude, DestinationLongitude, TotalVolume, 0, Keywords);

            if (list.Count() > 0)
                Counter.Hit(CurrentUser.Id, Counters.Match, ServiceId);

            return Success(new
            {
                List = list.Select(e => e.Json())
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
            var bytes = ListingSearchResult.ListingPDF(listing, StartDate, EndDate, Days, TransportDistance, Size, HireCost);
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
