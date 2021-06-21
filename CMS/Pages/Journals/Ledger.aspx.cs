using Agrishare.CMS.Code;
using Agrishare.Core.Entities;
using System;
using System.Linq;
using System.Text;

namespace Agrishare.CMS.Pages.Journals
{
    public partial class Ledger : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.Roles.Contains(Role.Administrator))
                Response.Redirect("/");

            var startDate = DateTime.Parse(Request.QueryString["StartDate"]);
            var endDate = DateTime.Parse(Request.QueryString["EndDate"]);
            var list = Journal.List(StartDate: startDate, EndDate: endDate, RegionId: CurrentRegion.Id);
            var balance = list.Count > 0 ? Journal.BalanceAt(list.First().Id, RegionId: CurrentRegion.Id) : 0;

            var csv = new StringBuilder();
            csv.AppendLine("Date,Description,Amount,Balance");
            foreach (var item in list)
            {
                item.Balance = balance;
                balance -= item.Amount;

                csv.Append(item.DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + ",");
                csv.Append(item.Title.Replace("\"", "\\\"") + ",");
                csv.Append(item.Amount.ToString("N0") + ",");
                csv.Append(item.Balance.ToString("N0"));
                csv.AppendLine();
            }

            var fileName = $"Ledger {startDate.ToString("yyyy-MM-dd")} - {endDate.ToString("yyyy-MM-dd")}.csv";
            var bytes = Encoding.ASCII.GetBytes(csv.ToString());
            Response.AddHeader("Content-Disposition", "Attachment; Filename=\"" + fileName + "\"");
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(bytes);
            Response.End();
        }
    }
}