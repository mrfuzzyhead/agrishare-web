using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Bus : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            if (Master.CurrentUser.PaymentMethods.Count == 0)
            {
                Master.Feedback = "Please update your accepted payment methods to continue with adding equipment";
                Response.Redirect("/account/profile/payments?r=/account/listing/bus");
            }

            if (!Page.IsPostBack)
            {
                var id = Convert.ToInt32(Request.QueryString["id"]);
                var listing = Core.Entities.Listing.Find(Id: id);
                if (listing.Id > 0)
                {
                    EquipmentTitle.Text = listing.Title;
                    Location.Latitude = listing.Latitude;
                    Location.Longitude = listing.Longitude;
                    Gallery.Photos = listing.Photos;

                    if (listing.Services.Count > 0)
                    {
                        TimePerQuantityUnit.Text = listing.Services.First().TimePerQuantityUnit.ToString();
                        DistanceCharge.Text = listing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = listing.Services.First().MaximumDistance.ToString();
                    }
                }

            }
        }

        public void Save(object s, EventArgs e)
        {
            var id = Convert.ToInt32(Request.QueryString["id"]);
            var listing = Core.Entities.Listing.Find(Id: id);

            if (listing.Id == 0)
            {
                listing.User = Master.CurrentUser;
                listing.CategoryId = Core.Entities.Category.BusId;
                listing.Services = new List<Core.Entities.Service>
                {
                    new Core.Entities.Service()
                };
            }

            listing.Title = EquipmentTitle.Text;
            listing.Latitude = Location.Latitude;
            listing.Longitude = Location.Longitude;
            listing.AvailableWithFuel = true;
            listing.AvailableWithoutFuel = true;

            var service = listing.Services.First();
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
                listing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (listing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.BusId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}