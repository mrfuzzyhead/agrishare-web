using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Land : Page
    {
        Core.Entities.Listing SelectedListing;
        private int CategoryId => Core.Entities.Category.LandId;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            if (Master.CurrentUser.PaymentMethods.Count == 0)
            {
                Master.Feedback = "Please update your accepted payment methods to continue with adding equipment";
                Response.Redirect("/account/profile/payments?r=/account/listing/land");
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
                LandRegion.Items.Add(new ListItem("Select...", ""));
                LandRegion.Items.Add(new ListItem($"{Core.Entities.ServiceLandRegion.Northern}", $"{(int)Core.Entities.ServiceLandRegion.Northern}"));
                LandRegion.Items.Add(new ListItem($"{Core.Entities.ServiceLandRegion.Eastern}", $"{(int)Core.Entities.ServiceLandRegion.Eastern}"));
                LandRegion.Items.Add(new ListItem($"{Core.Entities.ServiceLandRegion.Central}", $"{(int)Core.Entities.ServiceLandRegion.Central}"));
                LandRegion.Items.Add(new ListItem($"{Core.Entities.ServiceLandRegion.Western}", $"{(int)Core.Entities.ServiceLandRegion.Western}"));

                if (SelectedListing.Id > 0)
                {
                    EquipmentTitle.Text = SelectedListing.Title;
                    Location.Latitude = SelectedListing.Latitude;
                    Location.Longitude = SelectedListing.Longitude;
                    Gallery.Photos = SelectedListing.Photos;
                    GroupHire.Checked = SelectedListing.GroupServices;

                    if (SelectedListing.Services.Count > 0)
                    {
                        UnclearedLand.Checked = SelectedListing.Services.First().UnclearedLand;
                        ClearedLand.Checked = SelectedListing.Services.First().ClearedLand;
                        NearWaterSource.Checked = SelectedListing.Services.First().NearWaterSource;
                        FertileSoil.Checked = SelectedListing.Services.First().FertileSoil;
                        LandRegion.SelectedValue = $"{(int)SelectedListing.Services.First().LandRegion}";

                        PricePerQuantityUnit.Text = SelectedListing.Services.First().PricePerQuantityUnit.ToString();
                        MaxRentalYears.Text = SelectedListing.Services.First().MaxRentalYears.ToString();
                        AvailableAcres.Text = SelectedListing.Services.First().AvailableAcres.ToString();
                        MinimumAcres.Text = SelectedListing.Services.First().MinimumAcres.ToString();
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
            service.Mobile = false;
            service.CategoryId = CategoryId;
            service.UnclearedLand = UnclearedLand.Checked;
            service.ClearedLand = ClearedLand.Checked;
            service.NearWaterSource = NearWaterSource.Checked;
            service.FertileSoil = FertileSoil.Checked;
            service.LandRegion = (Core.Entities.ServiceLandRegion)Enum.ToObject(typeof(Core.Entities.ServiceLandRegion), Convert.ToInt32(LandRegion.SelectedValue));
            service.PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            service.QuantityUnitId = Core.Entities.QuantityUnit.Years;
            service.MaxRentalYears = Convert.ToInt32(MaxRentalYears.Text);
            service.AvailableAcres = Convert.ToDecimal(AvailableAcres.Text);
            service.MinimumAcres = Convert.ToDecimal(MinimumAcres.Text);

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