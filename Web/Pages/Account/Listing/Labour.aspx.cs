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
        private int CategoryId => Core.Entities.Category.LabourId;
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
                Services.DataSource = Core.Entities.Category.List(ParentId: CategoryId);
                Services.DataBind();

                if (SelectedListing.Id > 0)
                {
                    EquipmentTitle.Text = SelectedListing.Title;
                    GroupHire.Checked = SelectedListing.GroupServices;
                    Location.Latitude = SelectedListing.Latitude;
                    Location.Longitude = SelectedListing.Longitude;
                    Gallery.Photos = SelectedListing.Photos;

                    if (SelectedListing.Services.Count > 0)
                    {
                        PricePerDistanceUnit.Text = SelectedListing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = SelectedListing.Services.First().MaximumDistance.ToString();
                    }
                }

            }
        }

        public void BindService(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var category = (Core.Entities.Category)e.Item.DataItem;
                ((CheckBox)e.Item.FindControl("Title")).Text = category.Title;

                ((HiddenField)e.Item.FindControl("CategoryId")).Value = category.Id.ToString();
                ((HiddenField)e.Item.FindControl("ServiceId")).Value = "0";

                var service = SelectedListing.Services.FirstOrDefault(o => o.CategoryId == category.Id);
                if (service != null)
                {
                    ((CheckBox)e.Item.FindControl("Title")).Checked = true;
                    ((HiddenField)e.Item.FindControl("ServiceId")).Value = service.Id.ToString();
                    ((TextBox)e.Item.FindControl("PricePerQuantityUnit")).Text = service.PricePerQuantityUnit.ToString();
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
            }

            SelectedListing.Title = EquipmentTitle.Text;
            SelectedListing.GroupServices = GroupHire.Checked;
            SelectedListing.Latitude = Location.Latitude;
            SelectedListing.Longitude = Location.Longitude;

            var services = SelectedListing.Services;
            SelectedListing.Services = new List<Core.Entities.Service>();
            foreach (RepeaterItem item in Services.Items)
            {
                var enabled = ((CheckBox)item.FindControl("Title")).Checked;
                if (!enabled)
                    continue;

                var categoryId = Convert.ToInt32(((HiddenField)item.FindControl("CategoryId")).Value);
                var serviceId = Convert.ToInt32(((HiddenField)item.FindControl("ServiceId")).Value);
                var service = services.FirstOrDefault(o => o.Id == serviceId);

                if (service == null)
                    service = new Core.Entities.Service();

                service.CategoryId = categoryId;
                service.PricePerQuantityUnit = Convert.ToDecimal(((TextBox)item.FindControl("PricePerQuantityUnit")).Text);
                service.PricePerDistanceUnit = Convert.ToDecimal(PricePerDistanceUnit.Text);
                service.MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
                SelectedListing.Services.Add(service);
            }

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

        protected void ValidateServiceField(object source, ServerValidateEventArgs args)
        {
            //args.IsValid = false;
        }
    }
}