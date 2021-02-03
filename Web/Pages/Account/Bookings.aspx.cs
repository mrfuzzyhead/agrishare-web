using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Bookings : System.Web.UI.Page
    {
        private Core.Entities.NotificationGroup Group => (Core.Entities.NotificationGroup)Enum.Parse(typeof(Core.Entities.NotificationGroup), Request.QueryString["view"], true);
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            if (Group == Core.Entities.NotificationGroup.Offering)
            {
                List.RecordCount = Core.Entities.Booking.Count(SupplierId: Master.CurrentUser.Id);
                List.DataSource = Core.Entities.Booking.List(PageSize: 10, ListingUserId: Master.CurrentUser.Id, ListingSupplierId: Master.CurrentUser.SupplierId ?? 0);
            }
            else
            {
                List.RecordCount = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id);
                List.DataSource = Core.Entities.Booking.List(PageSize: 10, UserId: Master.CurrentUser.Id);
            }
            List.DataBind();

            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            MonthSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(UserId: Master.CurrentUser.Id, SupplierId: Master.CurrentUser.SupplierId ?? 0, StartDate: startDate).ToString("N2");
            AllTimeSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(UserId: Master.CurrentUser.Id, SupplierId: Master.CurrentUser.SupplierId ?? 0).ToString("N2");
        }

        public void BindBooking(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var booking = (Core.Entities.Booking)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/booking/details?id={booking.Id}";
                if ((booking.Listing.Photos?.Count ?? 0) > 0)
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{booking.Listing.Photos.FirstOrDefault().ThumbName}");
                ((Literal)e.Item.FindControl("Date")).Text = booking.StartDate.ToString("d MMMM yyyy");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(booking.Listing.Title);
                ((Literal)e.Item.FindControl("Price")).Text = "$" + booking.Price.ToString("N2");
            }
        }
    }
}