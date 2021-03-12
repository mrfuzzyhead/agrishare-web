using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Product : Page
    {
        Core.Entities.Product SelectedProduct;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            if (Master.CurrentUser.PaymentMethods.Count == 0)
            {
                Master.Feedback = "Please update your accepted payment methods to continue with adding equipment";
                Response.Redirect("/account/profile/payments?r=/account/listing/product");
            }

            var id = Convert.ToInt32(Request.QueryString["id"]);
            SelectedProduct = Core.Entities.Product.Find(Id: id);
            if (SelectedProduct.Id > 0 && SelectedProduct.Supplier.Id != Master.CurrentUser.Supplier.Id)
            {
                Master.Feedback = "Product not found";
                Response.Redirect("/account/offering");
            }

            if (!Page.IsPostBack)
            {
                if (SelectedProduct.Id > 0)
                {
                    EquipmentTitle.Text = SelectedProduct.Title;
                    EquipmentDescription.Text = SelectedProduct.Description;
                    Gallery.Photos = new List<Core.Entities.File> { SelectedProduct.Photo };
                    CostPerDay.Text = SelectedProduct.DayRate.ToString();
                    Stock.Text = SelectedProduct.Stock.ToString();
                }

            }
        }

        public void Save(object s, EventArgs e)
        {
            if (SelectedProduct.Id == 0)
                SelectedProduct.Supplier = Master.CurrentUser.Supplier;

            SelectedProduct.Title = EquipmentTitle.Text;
            SelectedProduct.Description = EquipmentDescription.Text;
            SelectedProduct.Photo = Gallery.Photos.Count > 0 ? Gallery.Photos.First() : null;
            SelectedProduct.DayRate = Convert.ToDecimal(CostPerDay.Text);
            SelectedProduct.Stock = Convert.ToInt32(Stock.Text);

            if (SelectedProduct.Save())
            {
                Master.Feedback = "Product updated";
                Response.Redirect($"/account/listings?cid=0");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}