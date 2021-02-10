using Agrishare.API;
using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSAdvertsController : BaseApiController
    {
        [Route("adverts/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Advert.Count(Keywords: Query, RegionId: CurrentRegion.Id);
            var list = Entities.Advert.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, RegionId: CurrentRegion.Id);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Advert.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Adverts"
            };

            return Success(data);
        }

        [Route("adverts/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var advert = Entities.Advert.Find(Id: Id);

            if (advert.Id == 0)
            {
                advert.StartDate = DateTime.Now;
                advert.EndDate = DateTime.Now.AddMonths(1);
                advert.RegionId = CurrentRegion.Id;
            }

            var data = new
            {
                Entity = advert.Json()
            };

            return Success(data);
        }

        [Route("adverts/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Advert Advert)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            Advert.StartDate = Advert.StartDate.AddMinutes(UTCOffset);
            Advert.EndDate = Advert.EndDate.AddMinutes(UTCOffset);

            if (Advert.Save())
                return Success(new
                {
                    Entity = Advert.Json()
                });

            return Error();
        }

        [Route("adverts/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var adverts = Entities.User.Find(Id: Id);

            if (adverts.Delete())
                return Success(new
                {
                    Entity = adverts.Json()
                });

            return Error();
        }
    }
}
