using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Irrigation : Page
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
                Response.Redirect("/account/profile/payments?r=/account/listing/irrigation");
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
                if (SelectedListing.Id > 0)
                {
                    EquipmentTitle.Text = SelectedListing.Title;
                    Location.Latitude = SelectedListing.Latitude;
                    Location.Longitude = SelectedListing.Longitude;
                    Gallery.Photos = SelectedListing.Photos;
                    GroupHire.Checked = SelectedListing.GroupServices;

                    if (SelectedListing.Services.Count > 0)
                    {
                        PricePerQuantityUnit.Text = SelectedListing.Services.First().PricePerQuantityUnit.ToString();
                        MaximumDistanceToWaterSource.Text = SelectedListing.Services.First().MaximumDistanceToWaterSource.ToString();
                        MaximumDepthOfWaterSource.Text = SelectedListing.Services.First().MaximumDepthOfWaterSource.ToString();

                        DistanceCharge.Text = SelectedListing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = SelectedListing.Services.First().MaximumDistance.ToString();
                    }
                }
            }
        }

        public void Save(object s, EventArgs e)
        {
            if (SelectedListing.Id == 0)
            {
                SelectedListing.User = Master.CurrentUser;
                SelectedListing.Region = Master.CurrentUser.Region;
                SelectedListing.CategoryId = Core.Entities.Category.IrrigationId;
                SelectedListing.Services = new List<Core.Entities.Service>
                {
                    new Core.Entities.Service()
                };
            }

            SelectedListing.Title = EquipmentTitle.Text;
            SelectedListing.GroupServices = GroupHire.Checked;
            SelectedListing.Latitude = Location.Latitude;
            SelectedListing.Longitude = Location.Longitude;
            SelectedListing.AvailableWithFuel = true;
            SelectedListing.AvailableWithoutFuel = true;

            var service = SelectedListing.Services.First();
            service.DistanceUnitId = Core.Entities.DistanceUnit.Km;
            service.FuelPerQuantityUnit = 0;
            service.MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            service.MinimumQuantity = 0;
            service.Mobile = true;
            service.PricePerDistanceUnit = Convert.ToDecimal(DistanceCharge.Text);
            service.QuantityUnitId = Core.Entities.QuantityUnit.Days;
            service.CategoryId = Core.Entities.Category.IrrigationId;
            service.PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            service.TimePerQuantityUnit = 1; // 1 Day
            service.TimeUnitId = Core.Entities.TimeUnit.None;
            service.PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            service.MaximumDistanceToWaterSource = Convert.ToDecimal(MaximumDistanceToWaterSource.Text);
            service.MaximumDepthOfWaterSource = Convert.ToDecimal(MaximumDepthOfWaterSource.Text);

            if (Gallery.Photos.Count > 0)
                SelectedListing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (SelectedListing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.IrrigationId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}