using Agrishare.Core;
using Agrishare.Core.Entities;
using System.Linq;
using System.Web.Http;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSProductsController : BaseApiController
    {
        [Route("products/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "", int SupplierId = 0)
        {
            var recordCount = Product.Count(Keywords: Query, SupplierId: SupplierId);
            var list = Product.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, SupplierId: SupplierId);

            var title = "Products";
            if (SupplierId > 0)
                title = Supplier.Find(SupplierId).Title;

            var data = new
            {
                Count = recordCount,
                Sort = Product.DefaultSort,
                List = list.Select(e => e.ListJson()),
                Title = title
            };

            return Success(data);
        }

        [Route("products/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0, int SupplierId = 0)
        {
            var suppliers = Supplier.List();

            var product = Product.Find(Id: Id);

            if (product.Id == 0)
                product.Supplier = suppliers.FirstOrDefault(e => e.Id == SupplierId) ?? suppliers.First();

            var data = new
            {
                Entity = product.Json(),
                Suppliers = suppliers.Select(e => new { e.Id, e.Title })
            };

            return Success(data);
        }

        [Route("products/save")]
        [AcceptVerbs("POST")]
        public object Save(Product Product)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Product.Save())
                return Success(new
                {
                    Entity = Product.Json()
                });

            return Error();
        }

        [Route("products/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var product = Product.Find(Id: Id);

            if (product.Delete())
                return Success(new
                {
                    Entity = product.Json()
                });

            return Error();
        }
    }
}
