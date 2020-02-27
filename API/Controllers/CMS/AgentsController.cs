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
    public class CMSAgentsController : BaseApiController
    {
        [Route("agents/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Agent.Count(Keywords: Query);
            var list = Entities.Agent.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);
            var bookingCounts = Agent.BookingCounts();

            foreach (var item in list)
                item.BookingCount = bookingCounts.FirstOrDefault(c => c.AgentId == item.Id)?.Count ?? 0;

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Agent.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Agents"
            };

            return Success(data);
        }

        [Route("agents/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var agent = Entities.Agent.Find(Id: Id);
            if (agent.Id > 0)
                agent.Commission = agent.Commission *= 100;

            var data = new
            {
                Entity = agent.Json(),
                Types = EnumInfo.ToList<AgentType>()
            };

            return Success(data);
        }

        [Route("agents/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Agent Agent)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Agent.Commission > 1)
                Agent.Commission /= 100;

            if (Agent.Save())
                return Success(new
                {
                    Entity = Agent.Json()
                });

            return Error();
        }

        [Route("agents/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var agents = Entities.User.Find(Id: Id);

            if (agents.Delete())
                return Success(new
                {
                    Entity = agents.Json()
                });

            return Error();
        }
    }
}
