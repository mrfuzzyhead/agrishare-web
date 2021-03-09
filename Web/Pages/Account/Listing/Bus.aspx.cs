using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Bus : Page
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
                Response.Redirect("/account/profile/payments?r=/account/listing/bus");
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

                    if (SelectedListing.Services.Count > 0)
                    {
                        TimePerQuantityUnit.Text = SelectedListing.Services.First().TimePerQuantityUnit.ToString();
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
                SelectedListing.CategoryId = Core.Entities.Category.BusId;
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

            var service = SelectedListing.Services.First();
            service.DistanceUnitId = Core.Entities.DistanceUnit.Km;
            service.FuelPerQuantityUnit = 0;
            service.MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            service.MinimumQuantity = 0;
            service.Mobile = true;
            service.PricePerDistanceUnit = Convert.ToDecimal(DistanceCharge.Text);
            service.QuantityUnitId = Core.Entities.QuantityUnit.None;
            service.CategoryId = Core.Entities.Category.BusServiceId;
            service.TimePerQuantityUnit = Convert.ToDecimal(TimePerQuantityUnit.Text);
            service.TimeUnitId = Core.Entities.TimeUnit.HoursPer100Kms;

            if (Gallery.Photos.Count > 0)
                SelectedListing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (SelectedListing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.BusId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}