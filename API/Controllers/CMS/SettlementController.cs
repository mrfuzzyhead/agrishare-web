using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSSettlementController : BaseApiController
    {
        [Route("settlement/report")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage SettlementReport()
        {
            var bookings = Entities.Booking.SettlementReport(StartDate: DateTime.Now.AddDays(-7), EndDate: DateTime.Now);

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

                    var recipientName = booking.User.Agent.TypeId == Entities.AgentType.Personal ? booking.User.Title : booking.User.Agent.Title;
                    var recipientTelephone = booking.User.Agent.TypeId == Entities.AgentType.Personal ? booking.User.Telephone : booking.User.Agent.Telephone;

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

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(csv.ToString());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = $"Settlement Report {DateTime.Now.ToString("yyyy-MM-dd")}.csv" };
            return result;
        }
    }
}
