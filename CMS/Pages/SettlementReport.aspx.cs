using Agrishare.CMS.Code;
using Agrishare.Core.Entities;
using System;
using System.Text;

namespace Agrishare.CMS.Pages
{
    public partial class SettlementReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.Roles.Contains(Role.Administrator))
                Response.Redirect("/");

            var bookings = Booking.SettlementReport(StartDate: DateTime.Now.AddDays(-7), EndDate: DateTime.Now);

            var csv = new StringBuilder();
            csv.AppendLine("Date,Booking,User,Telephone,Amount,Type,EcoCash Reference");
            foreach (var booking in bookings)
            {
                if (booking.User.Agent != null && booking.AgentCommission > 0)
                {
                    csv.Append(booking.StartDate.ToString("yyyy-MM-dd") + ",");
                    csv.Append(booking.Id.ToString() + ",");
                    csv.Append("\"" + booking.Listing.User.Title.Replace("\"", "\\\"") + "\",");
                    csv.Append(booking.Listing.User.Telephone + ",");
                    csv.Append((booking.Price - booking.AgriShareCommission).ToString() + ",");
                    csv.Append("Supplier,");
                    csv.AppendLine();

                    var recipientName = booking.User.Agent.TypeId == AgentType.Personal ? booking.User.Title : booking.User.Agent.Title;
                    var recipientTelephone = booking.User.Agent.TypeId == AgentType.Personal ? booking.User.Telephone : booking.User.Agent.Telephone;

                    csv.Append(booking.StartDate.ToString("yyyy-MM-dd") + ",");
                    csv.Append(booking.Id.ToString() + ",");
                    csv.Append("\"" + recipientName.Replace("\"", "\\\"") + "\",");
                    csv.Append(recipientTelephone + ",");
                    csv.Append((booking.AgriShareCommission * booking.AgentCommission).ToString() + ",");
                    csv.Append("Agent,");
                    csv.AppendLine();
                }
                else
                {
                    csv.Append(booking.StartDate.ToString("yyyy-MM-dd") + ",");
                    csv.Append(booking.Id.ToString() + ",");
                    csv.Append("\"" + booking.Listing.User.Title.Replace("\"", "\\\"") + "\",");
                    csv.Append(booking.Listing.User.Telephone + ",");
                    csv.Append((booking.Price - booking.AgriShareCommission).ToString() + ",");
                    csv.Append("Supplier,");
                    csv.AppendLine();
                }
            }

            var fileName = $"Settlement Report {DateTime.Now.ToString("yyyy-MM-dd")}.csv";
            var bytes = Encoding.ASCII.GetBytes(csv.ToString());
            Response.AddHeader("Content-Disposition", "Attachment; Filename=\"" + fileName + "\"");
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(bytes);
            Response.End();
        }
    }
}