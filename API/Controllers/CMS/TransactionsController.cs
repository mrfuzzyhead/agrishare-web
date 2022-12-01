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
    public class CMSTransactionsController : BaseApiController
    {
        [Route("transactions/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25)
        {
            var recordCount = Transaction.Count();
            var list = Transaction.List(PageIndex: PageIndex, PageSize: PageSize);

            var data = new
            {
                Count = recordCount,
                Sort = Transaction.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Transactions"
            };

            return Success(data);
        }

        [Route("transactions/status")]
        [AcceptVerbs("GET")]
        public object CheckStatus(int Id = 0)
        {
            var transaction = Transaction.Find(Id: Id);

            transaction.RequestStatus();

            var data = new
            {
                Entity = transaction.Json(),
                Feedback = $"Transaction status is {transaction.Status}"
            };

            return Success(data);
        }

        [Route("transactions/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var transaction = Transaction.Find(Id: Id);

            var data = new
            {
                Entity = transaction.DetailJson()
            };

            return Success(data);
        }
    }
}
