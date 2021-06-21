using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class ProductsController : BaseApiController
    {
        [Route("products/detail")]
        [AcceptVerbs("GET")]
        public object Details(int ProductId)
        {
            var product = Entities.Product.Find(Id: ProductId);

            if (product == null)
                return Error("Product not found");

            return Success(new
            {
                Listing = product.AppDetailJson ()        
            });
        }

        [Route("products/search")]
        [AcceptVerbs("GET")]
        public object ProductList(int PageIndex, int PageSize, string Query = "", int SupplierId = 0)
        {
            var count = Entities.Product.Count(SupplierId: SupplierId, Keywords: Query, RegionId: CurrentRegion.Id);
            var list = Entities.Product.List(PageIndex: PageIndex, PageSize: PageSize, SupplierId: SupplierId, Keywords: Query, RegionId: CurrentRegion.Id);

            var adverts = Entities.Advert.List(Live: true, PageSize: 5, Sort: "Random", RegionId: CurrentRegion.Id);
            Entities.Advert.AddImpressions(adverts.Select(e => e.Id).ToList());

            return Success(new
            {
                Count = count,
                List = list.Select(e => e.AppListJson()),
                Adverts = adverts.Select(e => e.AppJson())
            });
        }

        [Route("products/mine")]
        [AcceptVerbs("GET")]
        public object MyProductList(int PageIndex, int PageSize, string Query = "")
        {
            var count = Entities.Product.Count(SupplierId: CurrentUser.SupplierId ?? -1, Keywords: Query);
            var list = Entities.Product.List(PageIndex: PageIndex, PageSize: PageSize, SupplierId: CurrentUser.SupplierId ?? -1, Keywords: Query);

            return Success(new
            {
                Count = count,
                List = list.Select(e => e.AppListJson())
            });
        }

        [Route("products/available")]
        [AcceptVerbs("GET")]
        public object CheckAvailability([FromUri] ProductAvailabilityModel Model)
        {
            var productIds = Model.ProductIds.Split(',').Where(e => !string.IsNullOrEmpty(e)).Select(e => Convert.ToInt32(e)).ToList();
            var unavailableProducts = Entities.Product.Unavailable(productIds, Model.StartDate, Model.EndDate);

            if (unavailableProducts.Count == 0)
                return Success();

            var errorMessage = "The following items are not available between the selected dates: " + string.Join(", ", unavailableProducts.Select(e => e.Title));
            return Error(errorMessage);
        }

        [Route("products/availability")]
        [AcceptVerbs("GET")]
        public object Availability([FromUri] ProductAvailabilityModel Model)
        {
            var productIds = Model.ProductIds.Split(',').Where(e => !string.IsNullOrEmpty(e)).Select(e => Convert.ToInt32(e)).ToList();
            var bookings = Entities.Product.BookedDates(productIds, Model.StartDate, Model.EndDate);

            var list = new List<object>();

            var date = Model.StartDate;
            while (date <= Model.EndDate)
            {
                var start = date.StartOfDay();
                var end = date.AddDays(Model.Days - 1).EndOfDay();

                var available = bookings.Where(o =>
                    (o.StatusId == Entities.BookingStatus.Approved || o.StatusId == Entities.BookingStatus.InProgress || o.StatusId == Entities.BookingStatus.Pending || o.StatusId == Entities.BookingStatus.Incomplete) &&
                    o.StartDate <= end &&
                    o.EndDate >= start).Count() == 0;

                list.Add(new
                {
                    Date = date,
                    Available = available
                });

                date = date.AddDays(1);
            }

            return Success(new
            {
                Calendar = list
            });
        }
    }
}
