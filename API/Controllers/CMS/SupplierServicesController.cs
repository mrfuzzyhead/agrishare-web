using Agrishare.Core;
using Agrishare.Core.Entities;
using System.Linq;
using System.Web.Http;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSSupplierServicesController : BaseApiController
    {
        [Route("supplierservices/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Keywords = "", int SupplierId = 0)
        {
            var recordCount = SupplierService.Count(Keywords: Keywords, SupplierId: SupplierId);
            var list = SupplierService.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Keywords, SupplierId: SupplierId);

            var title = Supplier.Find(SupplierId).Title;

            var data = new
            {
                Count = recordCount,
                Sort = SupplierService.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("supplierservices/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0, int SupplierId = 0)
        {
            var supplierservice = SupplierService.Find(Id: Id);

            if (supplierservice.Id == 0)
            {
                supplierservice.Supplier = Supplier.Find(SupplierId);
            }

            var data = new
            {
                Entity = supplierservice.Json()
            };

            return Success(data);
        }

        [Route("supplierservices/save")]
        [AcceptVerbs("POST")]
        public object Save(SupplierService SupplierService)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (SupplierService.Save())
                return Success(new
                {
                    Entity = SupplierService.Json()
                });

            return Error();
        }

        [Route("supplierservices/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var supplierservice = SupplierService.Find(Id: Id);

            if (supplierservice.Delete())
                return Success(new
                {
                    Entity = supplierservice.Json()
                });

            return Error();
        }
    }
}
