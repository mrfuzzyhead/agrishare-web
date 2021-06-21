using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Labour : System.Web.UI.Page
    {
        private int CategoryId => Category.LabourId;
        Core.Entities.Listing SelectedListing;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            if (Master.CurrentUser.PaymentMethods.Count == 0)
            {
                Master.Feedback = "Please update your accepted payment methods to continue with adding equipment";
                Response.Redirect("/account/profile/payments?r=/account/listing/labour");
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
                Services.Items.Add(new ListItem("Harvesting", $"{(int)LabourService.Harvesting}"));
                Services.Items.Add(new ListItem("Land Clearing", $"{(int)LabourService.LandClearing}"));
                Services.Items.Add(new ListItem("Loading", $"{(int)LabourService.Loading}"));
                Services.Items.Add(new ListItem("Planting", $"{(int)LabourService.Planting}"));
                Services.Items.Add(new ListItem("Weeding", $"{(int)LabourService.Weeding}"));
                Services.Items.Add(new ListItem("Other", $"{(int)LabourService.Other}"));

                if (SelectedListing.Id > 0)
                {
                    EquipmentTitle.Text = SelectedListing.Title;
                    GroupHire.Checked = SelectedListing.GroupServices;
                    Location.Latitude = SelectedListing.Latitude;
                    Location.Longitude = SelectedListing.Longitude;
                    Gallery.Photos = SelectedListing.Photos;

                    if (SelectedListing.Services.Count > 0)
                    {
                        PricePerQuantityUnit.Text = SelectedListing.Services.First().PricePerQuantityUnit.ToString();
                        PricePerDistanceUnit.Text = SelectedListing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = SelectedListing.Services.First().MaximumDistance.ToString();

                        foreach (ListItem item in Services.Items)
                            if ((Convert.ToInt32(item.Value) & SelectedListing.Services.First().LabourServices) > 0)
                                item.Selected = true;
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
                SelectedListing.CategoryId = CategoryId;
                SelectedListing.Services = new List<Service>
                {
                    new Service()
                };
            }

            SelectedListing.Title = EquipmentTitle.Text;
            SelectedListing.GroupServices = GroupHire.Checked;
            SelectedListing.Latitude = Location.Latitude;
            SelectedListing.Longitude = Location.Longitude;
            SelectedListing.AvailableWithFuel = true;
            SelectedListing.AvailableWithoutFuel = true;

            var service = SelectedListing.Services.First();
            service.DistanceUnitId = DistanceUnit.Km;
            service.FuelPerQuantityUnit = 0;
            service.MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            service.MinimumQuantity = 0;
            service.Mobile = true;
            service.QuantityUnitId = QuantityUnit.Days;
            service.CategoryId = Category.LabourId;
            service.PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            service.TimeUnitId = TimeUnit.None;
            service.PricePerDistanceUnit = Convert.ToDecimal(PricePerDistanceUnit.Text);
            service.LabourServices = 0;
            foreach (ListItem item in Services.Items)
                if (item.Selected)
                    service.LabourServices += Convert.ToInt32(item.Value);

            if (Gallery.Photos.Count > 0)
                SelectedListing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (SelectedListing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={CategoryId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}