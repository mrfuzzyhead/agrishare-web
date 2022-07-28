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
    public class CMSFaqsController : BaseApiController
    {
        [Route("faqs/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "", Language LanguageId = Language.English)
        {
            var recordCount = Entities.Faq.Count(Keywords: Query, Language: LanguageId);
            var list = Entities.Faq.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, Language: LanguageId);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Faq.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "FAQs",
                Languages = EnumInfo.ToList<Entities.Language>(),
                Filter = new
                {
                    LanguageId
                }
            };

            return Success(data);
        }

        [Route("faqs/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Faq.Find(Id: Id).Json(),
                Languages = EnumInfo.ToList<Language>(),
                Regions = Region.List().Select(e => e.FaqsJson())
            };

            return Success(data);
        }

        [Route("faqs/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Faq Faq)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Faq.Save())
                return Success(new
                {
                    Entity = Faq.Json()
                });

            return Error();
        }

        [Route("faqs/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var faqs = Entities.Faq.Find(Id: Id);

            if (faqs.Delete())
                return Success(new
                {
                    Entity = faqs.Json()
                });

            return Error();
        }
    }
}
