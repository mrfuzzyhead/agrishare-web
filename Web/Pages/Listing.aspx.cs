using Agrishare.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages
{
    public partial class Listing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Body.Attributes["class"] += " account ";

            var selectedListing = Core.Entities.Listing.Find(Convert.ToInt32(Request.QueryString["id"]));

            if (selectedListing == null || selectedListing.Id == 0 || selectedListing.Deleted || selectedListing.StatusId == Core.Entities.ListingStatus.Hidden)
            {
                Master.Feedback = "Listing not found";
                Response.Redirect("/");
            }

            ListingTitle.Text = HttpUtility.HtmlEncode(selectedListing.Title);

            Reviews.CssClass = $"stars-{(int)selectedListing.AverageRating}";
            Reviews.Text = selectedListing.RatingCount == 0 ? "No reviews" : selectedListing.RatingCount == 1 ? "One review" : $"{selectedListing.RatingCount} reviews";
            Reviews.NavigateUrl = $"/account/listing/reviews?lid={selectedListing.Id}";

            Gallery.DataSource = selectedListing.Photos;
            Gallery.DataBind();

            ListingDescription.Text = HttpUtility.HtmlEncode(selectedListing.Description);
            Brand.Text = HttpUtility.HtmlEncode(selectedListing.Brand);
            HorsePower.Text = HttpUtility.HtmlEncode(selectedListing.HorsePower);
            Year.Text = HttpUtility.HtmlEncode(selectedListing.Year);
        }

        public void BindPhoto(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var photo = (Core.Entities.File)e.Item.DataItem;
                ((Image)e.Item.FindControl("Thumb")).ImageUrl = $"{Core.Entities.Config.CDNURL}/{photo.ZoomName}";
            }
        }
    }
}