using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Listings : Page
    {
        Core.Entities.Category Category;

        string Type
        {
            get
            {
                switch (Category?.Id ?? 0)
                {
                    case 0:
                        return "product";
                    case Core.Entities.Category.LorriesId:
                        return "lorry";
                    case Core.Entities.Category.TractorsId:
                        return "tractor";
                    case Core.Entities.Category.ProcessingId:
                        return "processing";
                    case Core.Entities.Category.BusId:
                        return "bus";
                    case Core.Entities.Category.IrrigationId:
                        return "irrigation";
                    case Core.Entities.Category.LabourId:
                        return "labour";
                    case Core.Entities.Category.LandId:
                        return "land";
                }
                return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            EquipmentMenuItem.Visible = Master.CurrentUser.Supplier != null;

            try { Category = Core.Entities.Category.Find(Id: int.Parse(Request.QueryString["cid"])); }
            catch { Category = null; }

            switch (Category?.Id ?? 0)
            {
                case 0:
                    ListTitle.Text = "Equipment"; break;
                case Core.Entities.Category.LorriesId:
                    ListTitle.Text = "Lorries"; break;
                case Core.Entities.Category.TractorsId:
                    ListTitle.Text = "Tractors"; break;
                case Core.Entities.Category.ProcessingId:
                    ListTitle.Text = "Processing"; break;
                case Core.Entities.Category.BusId:
                    ListTitle.Text = "Buses"; break;
                case Core.Entities.Category.IrrigationId:
                    ListTitle.Text = "Irrigation"; break;
                case Core.Entities.Category.LabourId:
                    ListTitle.Text = "Labour"; break;
                case Core.Entities.Category.LandId:
                    ListTitle.Text = "Land"; break;
            }

            if ((Category?.Id ?? 0) == 0 && Master.CurrentUser.Supplier != null)
            {
                ProductList.Visible = true;
                ProductList.RecordCount = Core.Entities.Product.Count(SupplierId: Master.CurrentUser.Supplier.Id);
                ProductList.DataSource = Core.Entities.Product.List(PageIndex: ProductList.CurrentPageIndex, PageSize: ProductList.PageSize, SupplierId: Master.CurrentUser.Supplier.Id);
                ProductList.DataBind();                
            }
            else if ((Category?.Id ?? 0) > 0)
            {
                ListingsList.Visible = true;
                ListingsList.RecordCount = Core.Entities.Listing.Count(UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
                ListingsList.DataSource = Core.Entities.Listing.List(PageIndex: ListingsList.CurrentPageIndex, PageSize: ListingsList.PageSize, UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
                ListingsList.DataBind();
            }
            else
            {
                Master.Feedback = "Category not found";
                Response.Redirect("/account/offering");
            }

            AddButton.Text = $"Add {Type}";
            AddButton.NavigateUrl = $"/account/listing/{Type}";
        }

        public void BindListing(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Core.Entities.Listing)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/listing/{Type}?id={listing.Id}";
                if ((listing.Photos?.Count ?? 0) > 0)
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{listing.Photos.FirstOrDefault().ThumbName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = HttpUtility.HtmlEncode(listing.Description);
            }
        }

        public void BindProduct(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Core.Entities.Product)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/listing/{Type}?id={listing.Id}";
                if (!string.IsNullOrEmpty(listing.Photo?.Filename))
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{listing.Photo.ThumbName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = $"{Master.CurrentUser.Region.Currency} {listing.DayRate.ToString("N2")}/day";
            }
        }
    }
}