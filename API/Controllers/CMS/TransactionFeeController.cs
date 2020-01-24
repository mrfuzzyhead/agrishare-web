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
    public class CMSTransactionFeesController : BaseApiController
    {
        [Route("transactionfees/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.TransactionFee.Count();
            var list = Entities.TransactionFee.List(PageIndex: PageIndex, PageSize: PageSize);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.TransactionFee.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Transaction Fees"
            };

            return Success(data);
        }

        [Route("transactionfees/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var transactionfee = Entities.TransactionFee.Find(Id: Id);

            var data = new
            {
                Entity = transactionfee.Json(),
                FeeTypes = EnumInfo.ToList<FeeType>().Where(g => g.Id != (int)FeeType.None).ToList()
            };

            return Success(data);
        }

        [Route("transactionfees/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.TransactionFee TransactionFee)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (TransactionFee.Save())
                return Success(new
                {
                    Entity = TransactionFee.Json()
                });

            return Error();
        }

        [Route("transactionfees/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var transactionfees = Entities.User.Find(Id: Id);

            if (transactionfees.Delete())
                return Success(new
                {
                    Entity = transactionfees.Json()
                });

            return Error();
        }
    }
}
