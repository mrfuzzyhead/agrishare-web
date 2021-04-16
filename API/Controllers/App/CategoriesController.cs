using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class CategoriesController : BaseApiController
    {
        [Route("categories/list")]
        [AcceptVerbs("GET")]
        public object List()
        {
            var list = new List<object>();

            var categories = Entities.Category.List();
            //var parents = Entities.Category.ParentListByListingCount();
            var parents = categories.Where(e => !e.ParentId.HasValue).ToList();
            var sortOrder = 1;
            foreach(var item in parents)
            {
                list.Add(new
                {
                    item.Id,
                    item.Title,
                    item.TitleShona,
                    item.TitleNdebele,
                    item.TitleLuganda,
                    Services = categories.Where(e => !e.Deleted && e.ParentId == item.Id).ToList().Select(e => new { e.Id, e.Title, e.TitleShona, e.TitleNdebele, e.TitleLuganda }),
                    SortOrder = sortOrder++
                });
            }

            return Success(new
            {
                List = list
            });
        }
    }
}
