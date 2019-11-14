using Agrishare.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Commission
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            if (Master.CurrentUser.AgentTypeId == Core.Entities.AgentUserType.Admin)
            {
                var bookings = Core.Entities.Booking.List(AgentId: Master.CurrentUser.AgentId.Value);

                var csv = new StringBuilder();
                csv.AppendLine("Date,Booking,User,Telephone,Amount");
                foreach (var booking in bookings)
                {
                    var recipientName = booking.User.Agent.TypeId == Core.Entities.AgentType.Personal ? booking.User.Title : booking.User.Agent.Title;
                    var recipientTelephone = booking.User.Agent.TypeId == Core.Entities.AgentType.Personal ? booking.User.Telephone : booking.User.Agent.Telephone;

                    csv.Append(booking.StartDate.ToString("yyyy-MM-dd") + ",");
                    csv.Append(booking.Id.ToString() + ",");
                    csv.Append("\"" + recipientName.Replace("\"", "\\\"") + "\",");
                    csv.Append(recipientTelephone + ",");
                    csv.Append((booking.AgriShareCommission * booking.AgentCommission).ToString() + ",");
                    csv.AppendLine();
                }

                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename=Commission Report {DateTime.Now.ToString("yyyy-MM-dd")}.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
                
        }
    }
}