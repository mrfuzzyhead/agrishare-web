using Agrishare.CMS.Code;
using Agrishare.Core;
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
            csv.AppendLine("Date,Transaction ID,Resource Category,Name Resource owner,Contact Number,Name of Resource user,Contact Number,Method of Payment,Total Amount paid,Reciept No. for amount recieved,Commission Generated,Agent Name,Amount Due to Agent,Commission/Revenue retained by Agrishare");
            foreach (var item in list)
            {
                item.Balance = balance;
                balance -= item.Amount;

                csv.Append(item.DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + ",");
                csv.Append(item.Id.ToString() + ",");

                var category = item.Booking?.Listing?.Category?.Title ?? "";
                csv.Append(CsvSafe(category) + ",");

                var owner = item.Booking?.Listing?.User?.Title ?? "";
                csv.Append(CsvSafe(owner) + ",");

                var ownerNumber = item.Booking?.Listing?.User?.Telephone ?? "";
                csv.Append(CsvSafe(ownerNumber) + ",");

                var user = item.User?.Title ?? "";
                csv.Append(CsvSafe(user) + ",");

                var userNumber = item.User?.Telephone ?? "";
                csv.Append(CsvSafe(userNumber) + ",");

                var gateway = $"{item.Gateway}".ExplodeCamelCase();
                csv.Append(CsvSafe(gateway) + ",");

                csv.Append(item.Amount.ToString("F2") + ",");
                csv.Append(CsvSafe(item.ReceiptNo) + ",");

                var agrishareCommission = item.Amount * item.AgrishareCommission;
                var agentCommission = item.Amount * item.AgentCommission;
                var totalCommission = agrishareCommission + agentCommission;
                csv.Append(totalCommission.ToString("F2") + ",");

                var agent = item.Booking?.Listing?.User?.Agent?.Title;
                csv.Append(CsvSafe(agent) + ",");
                csv.Append(agentCommission.ToString("F2") + ",");
                csv.Append(agrishareCommission.ToString("F2"));

                csv.AppendLine();
            }

            var fileName = $"Ledger {startDate.ToString("yyyy-MM-dd")} - {endDate.ToString("yyyy-MM-dd")}.csv";
            var bytes = Encoding.ASCII.GetBytes(csv.ToString());
            Response.AddHeader("Content-Disposition", "Attachment; Filename=\"" + fileName + "\"");
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(bytes);
            Response.End();
        }

        private string CsvSafe(string Text)
        {
            if (string.IsNullOrEmpty(Text))
                return string.Empty;
            return "\"" + Text.Replace("\"", "\\\"") + "\"";
        }
    }
}