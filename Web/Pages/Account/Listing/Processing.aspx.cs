using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Processing : Page
    {
        Core.Entities.Listing SelectedListing;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            if (Master.CurrentUser.PaymentMethods.Count == 0)
            {
                Master.Feedback = "Please update your accepted payment methods to continue with adding equipment";
                Response.Redirect("/account/profile/payments?r=/account/listing/processing");
            }

            var id = Convert.ToInt32(Request.QueryString["id"]);
            SelectedListing = Core.Entities.Listing.Find(Id: id);
            if (SelectedListing.Id > 0 && SelectedListing.User.Id != Master.CurrentUser.Id)
            {
                Master.Feedback = "Listing not found";
                Response.Redirect("/account/offering");
            }

            if (!Page.IsPostBack)
            {
                Service.Items.Add(new ListItem { Text = "Select...", Value = "" });
                var services = Core.Entities.Category.List(ParentId: Core.Entities.Category.ProcessingId);
                foreach (var service in services)
                    Service.Items.Add(new ListItem { Text = service.Title, Value = $"{service.Id}" });
            
                if (SelectedListing.Id > 0)
                {
                    EquipmentTitle.Text = SelectedListing.Title;
                    Location.Latitude = SelectedListing.Latitude;
                    Location.Longitude = SelectedListing.Longitude;
                    Gallery.Photos = SelectedListing.Photos;

                    if (SelectedListing.Services.Count > 0)
                    {
                        Service.SelectedValue = SelectedListing.Services.First().CategoryId.ToString();
                        TimePerQuantityUnit.Text = SelectedListing.Services.First().TimePerQuantityUnit.ToString();
                        PricePerQuantityUnit.Text = SelectedListing.Services.First().PricePerQuantityUnit.ToString();
                        MinimumQuantity.Text = SelectedListing.Services.First().MinimumQuantity.ToString("0");
                        Mobile.Checked = SelectedListing.Services.First().Mobile;
                        PricePerDistanceUnit.Text = SelectedListing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = SelectedListing.Services.First().MaximumDistance.ToString();
                    }
                }
                else
                {
                    PricePerDistanceUnit.Text = "0";
                    MaximumDistance.Text = "0";
                }

            }
        }

        public void Save(object s, EventArgs e)
        {
            if (SelectedListing.Id == 0)
            {
                SelectedListing.User = Master.CurrentUser;
                SelectedListing.Region = Master.CurrentUser.Region;
                SelectedListing.CategoryId = Core.Entities.Category.ProcessingId;
                SelectedListing.Services = new List<Core.Entities.Service>
                {
                    new Core.Entities.Service()
                };
            }

            SelectedListing.Title = EquipmentTitle.Text;
            SelectedListing.Latitude = Location.Latitude;
            SelectedListing.Longitude = Location.Longitude;
            SelectedListing.AvailableWithFuel = true;
            SelectedListing.AvailableWithoutFuel = true;

            SelectedListing.Services.First().DistanceUnitId = Core.Entities.DistanceUnit.Km;
            SelectedListing.Services.First().FuelPerQuantityUnit = 0;
            SelectedListing.Services.First().MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            SelectedListing.Services.First().MinimumQuantity = Convert.ToInt32(MinimumQuantity.Text);
            SelectedListing.Services.First().Mobile = Mobile.Checked;
            SelectedListing.Services.First().PricePerDistanceUnit = Convert.ToDecimal(PricePerDistanceUnit.Text);
            SelectedListing.Services.First().PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            SelectedListing.Services.First().QuantityUnitId = Core.Entities.QuantityUnit.Bags;
            SelectedListing.Services.First().CategoryId = Convert.ToInt32(Service.SelectedValue);
            SelectedListing.Services.First().TimePerQuantityUnit = Convert.ToDecimal(TimePerQuantityUnit.Text);
            SelectedListing.Services.First().TimeUnitId = Core.Entities.TimeUnit.BagsPerHour;
            SelectedListing.Services.First().TotalVolume = 0;

            if (Gallery.Photos.Count > 0)
                SelectedListing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (SelectedListing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.ProcessingId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}