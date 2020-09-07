using Agrishare.API;
using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsJournalsController : BaseApiController
    {
        [Route("journals/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Journal.Count();
            var list = Entities.Journal.List(PageIndex: PageIndex, PageSize: PageSize);
            var title = "Ledger";

            var balance = list.Count > 0 ? Entities.Journal.BalanceAt(list.First().Id) : 0;
            foreach (var item in list)
            {
                item.Balance = balance;
                balance -= item.Amount;
            }               

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("journals/reconcile/find")]
        [AcceptVerbs("GET")]
        public object FindReconcile(int Id = 0)
        {
            var data = new
            {
                Entity = new ReconcileLedgerModel(),
            };

            return Success(data);
        }

        [Route("journals/reconcile/save")]
        [AcceptVerbs("POST")]
        public object SaveReconcile(ReconcileLedgerModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var import = new Entities.JournalImport();
            import.ExcelFile = Model.ExcelFile;
            var success = import.Process(out var feedback);

            if (success)
                return Success(feedback);

            return Error(feedback);
        }

        [Route("journals/export/find")]
        [AcceptVerbs("GET")]
        public object FindExport(int Id = 0)
        {
            var data = new
            {
                Entity = new ExportLedgerModel(),
            };

            data.Entity.StartDate = DateTime.Now;
            while (data.Entity.StartDate.Day > 1)
                data.Entity.StartDate = data.Entity.StartDate.AddDays(-1);

            data.Entity.EndDate = DateTime.Now;
            while (data.Entity.EndDate.AddDays(1).Month == DateTime.Now.Month)
                data.Entity.EndDate = data.Entity.EndDate.AddDays(1);

            return Success(data);
        }

        [Route("journals/export/save")]
        [AcceptVerbs("POST")]
        public object Save(ExportLedgerModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            return Success(new
            {
                DownloadUrl = $"/Pages/Journals/Ledger.aspx?StartDate={Model.StartDate.ToString("yyyy-MM-dd")}&EndDate={Model.EndDate.ToString("yyyy-MM-dd")}"
            });
        }
    }

}
