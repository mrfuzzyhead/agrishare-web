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
    public class CMSTagsController : BaseApiController
    {
        [Route("tags/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Tag.Count(Keywords: Query);
            var list = Entities.Tag.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Tag.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Tags"
            };

            return Success(data);
        }

        [Route("tags/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Tag.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("tags/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Tag Tag)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Tag.Save())
                return Success(new
                {
                    Entity = Tag.Json()
                });

            return Error();
        }

        [Route("tags/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var tags = Entities.Tag.Find(Id: Id);

            if (tags.Delete())
                return Success(new
                {
                    Entity = tags.Json()
                });

            return Error();
        }
    }
}
