using Agrishare.Core;
using Agrishare.Core.Entities;
using System.Linq;
using System.Web.Http;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSSuppliersController : BaseApiController
    {
        [Route("suppliers/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Keywords = "")
        {
            var recordCount = Supplier.Count(Keywords: Keywords, RegionId: CurrentRegion.Id);
            var list = Supplier.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Keywords, RegionId: CurrentRegion.Id);

            var data = new
            {
                Count = recordCount,
                Sort = Supplier.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Suppliers"
            };

            return Success(data);
        }

        [Route("suppliers/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var supplier = Supplier.Find(Id: Id);

            if (supplier.Id == 0)
                supplier.Region = CurrentRegion;

            var data = new
            {
                Entity = supplier.Json()
            };

            return Success(data);
        }

        [Route("suppliers/save")]
        [AcceptVerbs("POST")]
        public object Save(Supplier Supplier)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Supplier.Save())
                return Success(new
                {
                    Entity = Supplier.Json()
                });

            return Error();
        }

        [Route("suppliers/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var supplier = Supplier.Find(Id: Id);

            if (supplier.Delete())
                return Success(new
                {
                    Entity = supplier.Json()
                });

            return Error();
        }
    }
}
