using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Products : System.Web.UI.Page
    {
        private Product SelectedProduct;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/seeking";
            Master.Body.Attributes["class"] += " account ";

            var id = Convert.ToInt32(Request.QueryString["id"]);
            if (Request.QueryString["book"] == "now")
            {
                BookNowPanel.Visible = true;
                if (Master.Cart.Count == 0)
                {
                    Master.Feedback = "Your cart is empty";
                    Response.Redirect("/account/seeking/products");
                }

                if (!Page.IsPostBack)
                {
                    For.Items.Add(new ListItem { Text = "Me", Value = ((int)Core.Entities.BookingFor.Me).ToString() });
                    For.Items.Add(new ListItem { Text = "A friend", Value = ((int)Core.Entities.BookingFor.Friend).ToString() });
                    For.Items.Add(new ListItem { Text = "A group", Value = ((int)Core.Entities.BookingFor.Group).ToString() });
                }
            }
            else if (id > 0)
            {
                SelectedProduct = Product.Find(id);
                if (SelectedProduct.Id == 0)
                {
                    Master.Feedback = "Product not found";
                    Response.Redirect("/account/seeking/products");
                }

                ProductDetails.Visible = true;
                Title = SelectedProduct.Title;
                ProductPageTitle.Text = HttpUtility.HtmlEncode(SelectedProduct.Title);
                HeroPhoto.Visible = !string.IsNullOrEmpty(SelectedProduct.Photo?.Filename);
                HeroPhoto.ImageUrl = $"{Config.CDNURL}/{SelectedProduct.Photo?.ZoomName}";
                DayRate.Text = $"{Master.CurrentUser.Region.Currency} {SelectedProduct.DayRate.ToString("N2")}/day";
                ProductDescription.Text = HttpUtility.HtmlEncode(SelectedProduct.Description);
            }
            else
            {
                var query = HttpUtility.UrlDecode(Request.QueryString["q"]);

                ProductPageTitle.Text = "Products";
                ProductList.Visible = true;
                ProductList.RecordCount = Product.Count(Keywords: query);
                ProductList.DataSource = Product.List(PageIndex: ProductList.CurrentPageIndex, PageSize: ProductList.PageSize, Keywords: query);
                ProductList.DataBind();

                ProductSearch.Visible = true;
                KeywordSearch.Attributes.Add("ng-init", $"keywords='{query}'");
            }

            CartList.RecordCount = Master.Cart.Count;
            CartList.DataSource = Master.Cart;
            CartList.DataBind();
        }

        public void BindProduct(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Core.Entities.Product)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/seeking/products?id={listing.Id}";
                if (!string.IsNullOrEmpty(listing.Photo?.Filename))
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{listing.Photo.ThumbName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = $"{Master.CurrentUser.Region.Currency} {listing.DayRate.ToString("N2")}/day";
            }
        }

        public void BindCartItem(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Product)e.Item.DataItem;
                if (!string.IsNullOrEmpty(listing.Photo?.Filename))
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{listing.Photo.ThumbName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = $"{Master.CurrentUser.Region.Currency} {listing.DayRate.ToString("N2")}/day";
                ((LinkButton)e.Item.FindControl("RemoveFromCartButton")).CommandArgument = e.Item.ItemIndex.ToString();
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (Request.QueryString["book"] == "now")
                    e.Item.FindControl("BookButton").Visible = false;
            }
        }

        public void AddToCart(object s, EventArgs e)
        {
            Master.Cart.Add(SelectedProduct);
            Master.Feedback = $"{SelectedProduct.Title} added to your cart";
            Response.Redirect("/account/seeking/products");
        }

        public void RemoveFromCart(object s, EventArgs e)
        {
            var button = (LinkButton)s;
            Master.Cart.RemoveAt(Convert.ToInt32(button.CommandArgument));
            Master.Feedback = $"Item removed from your cart";
            Response.Redirect("/account/seeking/products");
        }

        public void CreateBooking(object s, EventArgs e)
        {
            var productIds = Master.Cart.Select(product => product.Id).ToList();
            var dayCount = Math.Max(1, Convert.ToInt32(DayCount.Text));
            var startDate = DateTime.Parse(StartDate.Text);
            var endDate = startDate.AddDays(dayCount - 1);

            // check availability
            var unavailableProducts = Product.Unavailable(productIds, startDate, endDate);
            if (unavailableProducts.Count > 0)
            {
                Master.Feedback = "The following items are not available between the selected dates: " + string.Join(", ", unavailableProducts.Select(product => product.Title));
                Response.Redirect("/account/seeking/products?book=now");
            }

            // get product list and supplier list           
            var supplierList = new List<Supplier>();
            var productList = new List<Product>();
            foreach (var id in productIds)
            {
                var product = Product.Find(id);
                productList.Add(Product.Find(id));
                if (supplierList.Count(supplier => supplier.Id == product.SupplierId) == 0)
                    supplierList.Add(product.Supplier);
            }

            // create one booking per supplier
            var bookingList = new List<Core.Entities.Booking>();
            foreach (var supplier in supplierList)
            {
                var supplierProductList = productList.Where(product => product.SupplierId == supplier.Id).ToList();
                var hireCost = supplierProductList.Sum(product => product.DayRate) * dayCount * (1 + Core.Entities.Transaction.AgriShareCommission);
                var transportDistance = (int)Math.Ceiling(Core.Utils.Location.GetDistance(supplier.Longitude, supplier.Latitude, Location.Longitude, Location.Latitude) / 1000);
                var transportCost = transportDistance * 2 * supplier.TransportCostPerKm * dayCount;

                var booking = new Core.Entities.Booking
                {
                    ForId = Core.Entities.BookingFor.Me,
                    IncludeFuel = true,
                    Latitude = Location.Latitude,
                    Longitude = Location.Longitude,
                    Quantity = 1,
                    StartDate = startDate,
                    EndDate = endDate,
                    StatusId = Core.Entities.BookingStatus.Pending,
                    User = Master.CurrentUser,
                    AdditionalInformation = string.Empty,
                    HireCost = hireCost,
                    TransportCost = transportCost,
                    Price = hireCost + transportCost,
                    Distance = transportDistance,
                    TransportDistance = transportDistance,
                    Commission = Core.Entities.Transaction.AgriShareCommission,
                    AgentCommission = Master.CurrentUser.Agent?.Commission ?? 0,
                    IMTT = Core.Entities.Transaction.IMTT,
                    Products = supplierProductList,
                    Supplier = supplier
                };

                if (booking.Save())
                {
                    foreach (var product in booking.Products)
                        booking.AddProduct(product.Id);

                    var users = Core.Entities.User.List(SupplierId: supplier.Id);
                    foreach (var user in users)
                        new Core.Entities.Notification
                        {
                            Booking = booking,
                            GroupId = Core.Entities.NotificationGroup.Offering,
                            TypeId = Core.Entities.NotificationType.NewBooking,
                            User = user
                        }.Save(Notify: true);

                    new Core.Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Core.Entities.NotificationGroup.Seeking,
                        TypeId = Core.Entities.NotificationType.NewBooking,
                        User = Master.CurrentUser
                    }.Save(Notify: false);

                    bookingList.Add(booking);
                }

                Core.Entities.Counter.Hit(UserId: Master.CurrentUser.Id, Event: Core.Entities.Counters.Book, CategoryId: 0, BookingId: booking.Id);
            }

            if (bookingList.Count > 0)
            {
                Master.Feedback = "Your booking has been submitted";
                Response.Redirect("/account/bookings?view=seeking");
            }
            else
            {
                Master.Feedback = "Unable to create booking - an unknown error occurred";
                Response.Redirect("/account/seeking/products?book=now");
            }
        }
    }
}