using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSRegionsController : BaseApiController
    {
        [Route("regions/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Region.Count(Keywords: Query);
            var list = Entities.Region.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Region.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Regions"
            };

            return Success(data);
        }

        [Route("regions/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Region.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("regions/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Region Region)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Region.Save())
                return Success(new
                {
                    Entity = Region.Json()
                });

            return Error();
        }

        [Route("regions/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var regions = Entities.Region.Find(Id: Id);

            if (regions.Delete())
                return Success(new
                {
                    Entity = regions.Json()
                });

            return Error();
        }
    }
}
