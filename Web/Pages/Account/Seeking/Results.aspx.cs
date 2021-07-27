using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Results : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";
            Master.SelectedUrl = "/account/seeking";

            var categoryId = Convert.ToInt32(Request.QueryString["cid"]);
            var serviceId = Convert.ToInt32(Request.QueryString["sid"]);
            var latitude = Convert.ToDecimal(Request.QueryString["lat"]);
            var longitude = Convert.ToDecimal(Request.QueryString["lng"]);

            //var startDate = Convert.ToDateTime(Request.QueryString["std"]);
            DateTime? startDate = null;
            if (startDate == null)
                try { startDate = Convert.ToDateTime(Request.QueryString["std"]); } catch { }
            if (startDate == null)
                try { startDate = DateTime.ParseExact(Request.QueryString["std"], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); } catch { }
            if (startDate == null)
                startDate = DateTime.Today;
                        
            var size = Convert.ToInt32(Request.QueryString["qty"]);
            var includeFuel = Request.QueryString["fue"] == "1";
            var mobile = true;
            if (categoryId == Core.Entities.Category.ProcessingId)
                mobile = Request.QueryString["mob"] == "1";
            if (categoryId == Core.Entities.Category.LandId)
                mobile = false;
            var bookingFor = (Core.Entities.BookingFor)Enum.ToObject(typeof(Core.Entities.BookingFor), Convert.ToInt32(Request.QueryString["for"]));
            var destinationLatitude = Convert.ToDecimal(Request.QueryString["dla"]);
            var destinationLongitude = Convert.ToDecimal(Request.QueryString["dlo"]);
            var totalVolume = Convert.ToDecimal(Request.QueryString["vol"]);

            // new fields: irrigation, labour, land
            var distanceToWaterSource = Convert.ToDecimal(Request.QueryString["dis"]);
            var depthOfWaterSource = Convert.ToDecimal(Request.QueryString["dep"]);
            var labourServices = Convert.ToInt32(Request.QueryString["lsr"]);
            var landRegion = Convert.ToInt32(Request.QueryString["reg"]);

            Core.Entities.Counter.Hit(UserId: Master.CurrentUser.Id, Event: Core.Entities.Counters.Search, CategoryId: categoryId);

            SearchResults.RecordCount = Core.Entities.ListingSearchResult.Count(
                CategoryId: categoryId, ServiceId: serviceId, Latitude: latitude, Longitude: longitude, StartDate: startDate.Value, Size: size,
                IncludeFuel: includeFuel, Mobile: mobile, For: bookingFor, DestinationLatitude: destinationLatitude, DestinationLongitude: destinationLongitude,
                TotalVolume: totalVolume, RegionId: Master.CurrentUser.Region.Id, DistanceToWaterSource: distanceToWaterSource, DepthOfWaterSource: depthOfWaterSource,
                LabourServices: labourServices, LandRegion: landRegion);

            SearchResults.DataSource = Core.Entities.ListingSearchResult.List(PageIndex: SearchResults.CurrentPageIndex, PageSize: SearchResults.PageSize, 
                Sort: "Distance", CategoryId: categoryId, ServiceId: serviceId, Latitude: latitude, Longitude: longitude, StartDate: startDate.Value, Size: size,
                IncludeFuel: includeFuel, Mobile: mobile, For: bookingFor, DestinationLatitude: destinationLatitude, DestinationLongitude: destinationLongitude, 
                TotalVolume: totalVolume, RegionId: Master.CurrentUser.Region.Id, DistanceToWaterSource: distanceToWaterSource, DepthOfWaterSource: depthOfWaterSource,
                LabourServices: labourServices, LandRegion: landRegion);

            SearchResults.DataBind();

            if (SearchResults.CurrentPageIndex == 0 && SearchResults.RecordCount > 0)
                Core.Entities.Counter.Hit(UserId: Master.CurrentUser.Id, Event: Core.Entities.Counters.Match, CategoryId: categoryId);
        }

        public void BindSearchResult(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var result = (Core.Entities.ListingSearchResult)e.Item.DataItem;

                if ((result.Photos?.Count ?? 0) > 0)
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{result.Photos.FirstOrDefault().ThumbName})");

                if (result.Distance < 0)
                    ((Literal)e.Item.FindControl("Distance")).Text = "";
                else
                    ((Literal)e.Item.FindControl("Distance")).Text = Math.Round(result.Distance) == 0 ? "Nearby" : $"{Math.Round(result.Distance)}kms away";

                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(result.Title);
                ((Literal)e.Item.FindControl("Price")).Text = Master.CurrentCurrency + result.Price.ToString("N2");
                ((Literal)e.Item.FindControl("Status")).Text = result.Available ? "Available" : "Not available";

                var qs = new NameValueCollection(Request.QueryString)
                {
                    { "lid", result.ListingId.ToString() }
                };
                var str = string.Join("&", qs.AllKeys.ToList().Select(k => $"{k}={HttpUtility.UrlEncode(qs[k])}"));
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/booking/details?{str}";

            }
        }
    }
}