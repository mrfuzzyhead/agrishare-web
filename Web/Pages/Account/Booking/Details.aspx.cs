﻿using Agrishare.Core;
using Agrishare.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Booking
{
    public partial class Details : System.Web.UI.Page
    {
        private Core.Entities.Booking SelectedBooking;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            if (Request.QueryString["id"] != null)
                SetupBooking();
            else if (Request.QueryString["lid"] != null)
                SetupRequest();
        }

        private void SetupBooking()
        {
            try { SelectedBooking = Core.Entities.Booking.Find(Id: Convert.ToInt32(Request.QueryString["id"])); }
            catch { SelectedBooking = null; }

            if (SelectedBooking == null || (SelectedBooking.UserId != Master.CurrentUser.Id && SelectedBooking.Listing.UserId != Master.CurrentUser.Id))
            {
                Master.Feedback = "Booking not found";
                Response.Redirect("/account/seeking");
            }

            Master.SelectedUrl = SelectedBooking.UserId == Master.CurrentUser.Id ? "/account/seeking" : "/account/offering";

            ShowDetails();
        }

        private void SetupRequest()
        {
            Master.SelectedUrl = "/account/seeking";

            var listingId = Convert.ToInt32(Request.QueryString["lid"]);
            var categoryId = Convert.ToInt32(Request.QueryString["cid"]);
            var serviceId = Convert.ToInt32(Request.QueryString["sid"]);
            var latitude = Convert.ToDecimal(Request.QueryString["lat"]);
            var longitude = Convert.ToDecimal(Request.QueryString["lng"]);
            var startDate = Convert.ToDateTime(Request.QueryString["std"]);
            var size = Convert.ToInt32(Request.QueryString["qty"]);
            var includeFuel = Request.QueryString["fue"] == "1";
            var mobile = true;
            if (categoryId == Category.ProcessingId)
                mobile = Request.QueryString["mob"] == "1";
            if (categoryId == Core.Entities.Category.LandId)
                mobile = false;
            var bookingFor = (Core.Entities.BookingFor)Enum.ToObject(typeof(Core.Entities.BookingFor), Convert.ToInt32(Request.QueryString["for"]));
            var destinationLatitude = Convert.ToDecimal(Request.QueryString["dla"]);
            var destinationLongitude = Convert.ToDecimal(Request.QueryString["dlo"]);
            var totalVolume = Convert.ToDecimal(Request.QueryString["vol"]);
            var additionalInfo = Request.QueryString["des"];
            
            // new fields: irrigation, labour, land
            var distanceToWaterSource = Convert.ToDecimal(Request.QueryString["dis"]);
            var depthOfWaterSource = Convert.ToDecimal(Request.QueryString["dep"]);
            var labourServices = Convert.ToInt32(Request.QueryString["lsr"]);
            var landRegion = Convert.ToInt32(Request.QueryString["reg"]);

            var results = ListingSearchResult.List(PageIndex: 0, PageSize: 2, ListingId: listingId,
                Sort: "Distance", CategoryId: categoryId, ServiceId: serviceId, Latitude: latitude, Longitude: longitude, StartDate: startDate, Size: size,
                IncludeFuel: includeFuel, Mobile: mobile, For: bookingFor, DestinationLatitude: destinationLatitude, DestinationLongitude: destinationLongitude,
                TotalVolume: totalVolume, RegionId: Master.CurrentUser.Region.Id, DistanceToWaterSource: distanceToWaterSource, DepthOfWaterSource: depthOfWaterSource,
                LabourServices: labourServices, LandRegion: landRegion);

            if (results.Count != 1)
            {
                Master.Feedback = "Listing not found";
                Response.Redirect("/account/seeking");
            }
            var result = results.First();

            SelectedBooking = new Core.Entities.Booking
            {
                AdditionalInformation = additionalInfo,
                BookingUsers = null,
                DateCreated = DateTime.Now,
                Deleted = false,
                Destination = string.Empty,
                DestinationLatitude = destinationLongitude,
                DestinationLongitude = destinationLatitude,
                Distance = (decimal)result.Distance,
                EndDate = result.EndDate,
                ForId = bookingFor,
                FuelCost = (decimal)result.FuelCost,
                HireCost = (decimal)result.HireCost,
                Id = 0,
                IncludeFuel = includeFuel,
                LastModified = DateTime.Now,
                Latitude = latitude,
                Listing = Core.Entities.Listing.Find(Id: result.ListingId),
                ListingId = result.ListingId,
                Location = string.Empty,
                Longitude = longitude,
                Price = (decimal)result.Price,
                Quantity = size,
                Service = Core.Entities.Service.Find(Id: result.ServiceId),
                ServiceId = result.ServiceId,
                StartDate = result.StartDate,
                StatusId = Core.Entities.BookingStatus.None,
                Transactions = null,
                TotalVolume = totalVolume,
                TransportCost = (decimal)result.TransportCost,
                TransportDistance = (decimal)result.TransportDistance,
                User = Master.CurrentUser,
                UserId = Master.CurrentUser.Id,
                Commission = Core.Entities.Transaction.AgriShareCommission
            };

            ShowDetails(result.Available);
        }

        private void ShowDetails(bool Available = true)
        {
            BookingTitle.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing?.Title ?? SelectedBooking.Supplier?.Title ?? "Booking");

            if (SelectedBooking.Listing != null)
            {
                Reviews.CssClass = $"stars-{(int)SelectedBooking.Listing.AverageRating}";
                Reviews.Text = SelectedBooking.Listing.RatingCount == 0 ? "No reviews" : SelectedBooking.Listing.RatingCount == 1 ? "One review" : $"{SelectedBooking.Listing.RatingCount} reviews";
                Reviews.NavigateUrl = $"/account/listing/reviews?lid={SelectedBooking.ListingId}";

                Gallery.DataSource = SelectedBooking.Listing.Photos;
                Gallery.DataBind();

                Description.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Description);
                Brand.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Brand);
                HorsePower.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.HorsePower);
                Year.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Year);

                CommissionRow.Visible = SelectedBooking.Listing.UserId == Master.CurrentUser.Id;
            }           
            else
            {
                CommissionRow.Visible = SelectedBooking.Supplier.Id == Master.CurrentUser.Supplier?.Id;
                FuelCostRow.Visible = false;

                ProductList.Visible = true;
                ProductList.RecordCount = SelectedBooking.Products.Count;
                ProductList.DataSource = SelectedBooking.Products;
                ProductList.DataBind();
            }

            if (SelectedBooking.StartDate.Date == SelectedBooking.EndDate.Date)
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy");
            else if (SelectedBooking.StartDate.ToString("MM/yy") == SelectedBooking.EndDate.ToString("MM/yy"))
                Dates.Text = SelectedBooking.StartDate.Day + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");
            else
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy") + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");

            var days = (SelectedBooking.EndDate - SelectedBooking.StartDate).TotalDays + 1;
            if (days >= 365)
            {
                var years = (int)Math.Round(days / 365);
                Days.Text = years == 1 ? "1 year" : $"{years} years";
            }
            else
            {
                Days.Text = days == 1 ? "1 day" : $"{days} days";
            }

            AvailabilityDays.Attributes.Add("ng-init", $"calendar.days={days};calendar.volume={SelectedBooking.TotalVolume};");
            ListingId.Attributes.Add("ng-init", $"calendar.listingId={SelectedBooking.ListingId}");
            StartDate.Attributes.Add("ng-init", $"calendar.startDate=calendar.date('{SelectedBooking.StartDate.ToString("yyy-MM-dd")}')");
            Availability.Visible = SelectedBooking.Id == 0; // !Available;
            Unavaiable.Visible = !Available;

            TransportDistance.Text = SelectedBooking.TransportDistance.ToString("N2") + " km";
            TransportCost.Text = Master.CurrentCurrency + SelectedBooking.TransportCost.ToString("N2");
            HireCost.Text = Master.CurrentCurrency + SelectedBooking.HireCost.ToString("N2");

            if (SelectedBooking.Listing != null)
            {
                HireSize.Text = SelectedBooking.Quantity + " " + SelectedBooking.Service.QuantityUnit;
                if (SelectedBooking.Listing.CategoryId == Category.LorriesId)
                    FuelSize.Text = SelectedBooking.TransportDistance.ToString("N2") + "km";
                else
                    FuelSize.Text = SelectedBooking.Quantity + " " + SelectedBooking.Service.DistanceUnit;
                FuelCost.Text = Master.CurrentCurrency + SelectedBooking.FuelCost.ToString("N2");

                HireCostRow.Visible = SelectedBooking.Listing.CategoryId != Category.BusId;
                FuelCostRow.Visible = SelectedBooking.Listing.CategoryId != Category.BusId && SelectedBooking.Listing.CategoryId != Category.IrrigationId && SelectedBooking.Listing.CategoryId != Category.LabourId && SelectedBooking.Listing.CategoryId != Category.LandId;
            }
            else
            {
                HireSize.Text = SelectedBooking.Products.Count == 1 ? "1 product" : $"{SelectedBooking.Products.Count} products";
            }

            Commission.Text = Master.CurrentCurrency + SelectedBooking.AgriShareCommission.ToString("N2");

            Total.Text = Master.CurrentCurrency + SelectedBooking.Price.ToString("N2");

            /** Land **/

            if (SelectedBooking.Listing?.CategoryId == Category.LandId)
            {
                TransportCostRow.Visible = false;
                FuelCostRow.Visible = false;
            }

            /******************************/

            var isSeeker = SelectedBooking.UserId == Master.CurrentUser.Id;

            var isOfferer =
                SelectedBooking.Listing != null ? SelectedBooking.Listing.UserId == Master.CurrentUser.Id : SelectedBooking.Supplier.Id == Master.CurrentUser.Supplier?.Id;
            
            RequestPanel.Visible = isSeeker && SelectedBooking.Id == 0 && Available;
            AwaitingConfirmPanel.Visible = isSeeker && SelectedBooking.StatusId == Core.Entities.BookingStatus.Pending;
            PendingPanel.Visible = isOfferer && SelectedBooking.StatusId == Core.Entities.BookingStatus.Pending;
            DeclinedPanel.Visible = SelectedBooking.StatusId == Core.Entities.BookingStatus.Declined;
            AwaitingPaymentPanel.Visible = isOfferer && SelectedBooking.StatusId == Core.Entities.BookingStatus.Approved;
            
            PaidPanel.Visible = isOfferer && SelectedBooking.StatusId == Core.Entities.BookingStatus.InProgress;
            InProgressPanel.Visible = isSeeker && SelectedBooking.StatusId == Core.Entities.BookingStatus.InProgress;
            IncompletePanel.Visible = isOfferer && SelectedBooking.StatusId == Core.Entities.BookingStatus.Incomplete;
            CompletePanel.Visible = SelectedBooking.StatusId == Core.Entities.BookingStatus.Complete;
            CancelledPanel.Visible = SelectedBooking.StatusId == Core.Entities.BookingStatus.Cancelled;

            if (isSeeker && SelectedBooking.StatusId == Core.Entities.BookingStatus.Approved)
            {
                var bookingUsers = Core.Entities.BookingUser.List(BookingId: SelectedBooking.Id);
                if (bookingUsers == null || bookingUsers.Select(o => o.Ratio).DefaultIfEmpty(0).Sum() < 1)
                {
                    PaymentPanel.Visible = isSeeker && SelectedBooking.StatusId == Core.Entities.BookingStatus.Approved && SelectedBooking.ForId != Core.Entities.BookingFor.Group;
                    GroupPaymentPanel.Visible = isSeeker && SelectedBooking.StatusId == Core.Entities.BookingStatus.Approved && SelectedBooking.ForId == Core.Entities.BookingFor.Group;

                    if (bookingUsers?.Count > 0)
                    {
                        var arr = bookingUsers.Select(e => new BookingUserModel { Id = e.Id, Name = e.Name, Quantity = e.Ratio * SelectedBooking.Quantity, Telephone = e.Telephone });
                        BookingUsers.Attributes.Add("ng-init", "group.users=" + JsonConvert.SerializeObject(arr));
                    }
                    else
                        BookingUsers.Attributes.Add("ng-init", "group.users=[{},{}]");
                }
                else
                {
                    PaymentProgressPanel.Visible = true;
                    PaymentProgressPanel.Attributes.Add("ag-booking-id", SelectedBooking.Id.ToString());
                }
                
            }

            if (PaymentPanel.Visible && SelectedBooking.ForId == BookingFor.Me)
            {
                //PayerMobileNumber.Text = Master.CurrentUser.Title;
                //PayerMobileNumber.Text = Master.CurrentUser.Telephone;

                if (SelectedBooking.Listing.User.PaymentMethods.Contains(PaymentMethod.Cash))
                {
                    PaymentCashPanel.Visible = true;
                    CashDeliveryAddress.Text = Config.AgriShareOfficeLocation.Replace(@"\n", "<br/>");
                }

                if (SelectedBooking.Listing.User.PaymentMethods.Contains(PaymentMethod.BankTransfer))
                {
                    PaymentBankPanel.Visible = true;
                    BankDetails.Text = Config.AgriShareBankDetails.Replace(@"\n", "<br/>");
                }

                if (SelectedBooking.Listing.User.PaymentMethods.Contains(PaymentMethod.Cash) &&
                    SelectedBooking.Listing.User.PaymentMethods.Contains(PaymentMethod.BankTransfer))
                {
                    PaymentOrPanel.Visible = true;
                }

                if (SelectedBooking.Listing.User.PaymentMethods.Contains(PaymentMethod.MobileMoney))
                {
                    MobileMoneyPanel.Visible = true;
                }

            }

            if (CompletePanel.Visible)
            {
                var rating = Core.Entities.Rating.Find(BookingId: SelectedBooking.Id);
                if (rating.Id > 0)
                {
                    Review.Visible = true;
                    ReviewStars.Attributes.Add("class", $"stars-{(int)rating.Stars}");
                    ReviewUser.Text = HttpUtility.HtmlEncode(rating.User.FirstName);
                    ReviewComments.Text = HttpUtility.HtmlEncode(rating.Comments);
                    ReviewDate.Text = rating.DateCreated.ToString("d MMMM yyyy");
                }
            }
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

        public void BindReview(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var review = (Core.Entities.Rating)e.Item.DataItem;
                ((HtmlControl)e.Item.FindControl("Stars")).Attributes.Add("class", $"stars-{(int)review.Stars}");
                ((Literal)e.Item.FindControl("User")).Text = HttpUtility.HtmlEncode(review.User.FirstName);
                ((Literal)e.Item.FindControl("Comments")).Text = HttpUtility.HtmlEncode(review.Comments);
                ((Literal)e.Item.FindControl("Date")).Text = review.DateCreated.ToString("d MMMM yyyy");
            }
        }

        public void BindPhoto(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var photo = (Core.Entities.File)e.Item.DataItem;
                ((Image)e.Item.FindControl("Thumb")).ImageUrl = $"{Core.Entities.Config.CDNURL}/{photo.ZoomName}";
            }
        }

        /**************/

        public void SendRequest(object s, EventArgs e)
        {
            SelectedBooking.StatusId = Core.Entities.BookingStatus.Pending;
            if (SelectedBooking.Save())
            {
                Core.Entities.Counter.Hit(UserId: Master.CurrentUser.Id, Event: Core.Entities.Counters.Book, CategoryId: SelectedBooking.Service.CategoryId, BookingId: SelectedBooking.Id);

                new Core.Entities.Notification
                {
                    Booking = SelectedBooking,
                    GroupId = Core.Entities.NotificationGroup.Offering,
                    TypeId = Core.Entities.NotificationType.NewBooking,
                    User = Core.Entities.User.Find(Id: SelectedBooking.Listing.UserId)
                }.Save(Notify: true);

                new Core.Entities.Notification
                {
                    Booking = SelectedBooking,
                    GroupId = Core.Entities.NotificationGroup.Seeking,
                    TypeId = Core.Entities.NotificationType.NewBooking,
                    User = Master.CurrentUser
                }.Save(Notify: false);

                Master.Feedback = "Your request has been sent";
                Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
            }

            Master.Feedback = "There was an error sending your request - please try again";
        }

        public void ConfirmBooking(object s, EventArgs e)
        {
            if (SelectedBooking == null || SelectedBooking.Listing.UserId != Master.CurrentUser.Id)
                Master.Feedback = "Booking not found";

            else if (SelectedBooking.StatusId != Core.Entities.BookingStatus.Pending)
                Master.Feedback = "This booking has already been updated";

            else
            {
                SelectedBooking.StatusId = Core.Entities.BookingStatus.Approved;
                if (SelectedBooking.Save())
                {
                    var notifications = Core.Entities.Notification.List(BookingId: SelectedBooking.Id, Type: Core.Entities.NotificationType.NewBooking);
                    foreach (var notification in notifications)
                    {
                        notification.StatusId = Core.Entities.NotificationStatus.Complete;
                        notification.Save();
                    }

                    new Core.Entities.Notification
                    {
                        Booking = SelectedBooking,
                        GroupId = Core.Entities.NotificationGroup.Seeking,
                        TypeId = Core.Entities.NotificationType.BookingConfirmed,
                        User = Core.Entities.User.Find(Id: SelectedBooking.UserId)
                    }.Save(Notify: true);

                    Core.Entities.Counter.Hit(UserId: SelectedBooking.UserId, Event: Core.Entities.Counters.ConfirmBooking, CategoryId: SelectedBooking.Service.CategoryId, BookingId: SelectedBooking.Id);

                    Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
                }

                Master.Feedback = "There was an error sending your request - please try again";
            }
        }

        public void DeclineBooking(object s, EventArgs e)
        {
            if (SelectedBooking == null || SelectedBooking.Listing.UserId != Master.CurrentUser.Id)
                Master.Feedback = "Booking not found";

            else if (SelectedBooking.StatusId != Core.Entities.BookingStatus.Pending)
                Master.Feedback = "This SelectedBooking has already been updated";

            else
            {

                SelectedBooking.StatusId = Core.Entities.BookingStatus.Declined;
                if (SelectedBooking.Save())
                {
                    var notifications = Core.Entities.Notification.List(BookingId: SelectedBooking.Id, Type: Core.Entities.NotificationType.NewBooking);
                    foreach (var notification in notifications)
                    {
                        notification.StatusId = Core.Entities.NotificationStatus.Complete;
                        notification.Save();
                    }

                    new Core.Entities.Notification
                    {
                        Booking = SelectedBooking,
                        GroupId = Core.Entities.NotificationGroup.Seeking,
                        TypeId = Core.Entities.NotificationType.BookingCancelled,
                        User = Core.Entities.User.Find(Id: SelectedBooking.UserId)
                    }.Save(Notify: true);

                    Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
                }

                Master.Feedback = "There was an error sending your request - please try again";
            }
        }

        public void SubmitRating(object s, EventArgs e)
        {
            if (SelectedBooking == null || SelectedBooking.UserId != Master.CurrentUser.Id)
                Master.Feedback = "Booking not found";

            else if (!SelectedBooking.ListingId.HasValue)
                Master.Feedback = "Booking does not have a linked listing";

            else
            {
                var rating = new Core.Entities.Rating
                {
                    BookingId = SelectedBooking.Id,
                    Comments = RatingComments.Text,
                    ListingId = SelectedBooking.ListingId.Value,
                    Stars = Convert.ToInt32(RatingStars.Text),
                    User = Master.CurrentUser
                };

                if (rating.Save())
                {
                    SelectedBooking.StatusId = Core.Entities.BookingStatus.Complete;
                    if (SelectedBooking.Save())

                    {
                        new Core.Entities.Notification
                        {
                            Booking = SelectedBooking,
                            GroupId = Core.Entities.NotificationGroup.Offering,
                            TypeId = Core.Entities.NotificationType.ServiceComplete,
                            User = Core.Entities.User.Find(Id: SelectedBooking.Listing.UserId)
                        }.Save(Notify: true);

                        Core.Entities.Counter.Hit(UserId: SelectedBooking.UserId, Event: Core.Entities.Counters.CompleteBooking, CategoryId: SelectedBooking.Service.CategoryId, BookingId: SelectedBooking.Id);

                        var listing = Core.Entities.Listing.Find(Id: rating.ListingId);
                        listing.AverageRating = ((listing.AverageRating * listing.RatingCount) + rating.Stars) / (listing.RatingCount + 1);
                        listing.RatingCount += 1;
                        listing.Save();

                        var notifications = Core.Entities.Notification.List(Type: Core.Entities.NotificationType.ServiceComplete, UserId: SelectedBooking.UserId);
                        foreach (var notification in notifications)
                        {
                            notification.StatusId = Core.Entities.NotificationStatus.Complete;
                            notification.Save();
                        }

                        new Core.Entities.Notification
                        {
                            Booking = SelectedBooking,
                            GroupId = Core.Entities.NotificationGroup.Offering,
                            TypeId = Core.Entities.NotificationType.NewReview,
                            User = Core.Entities.User.Find(Id: listing.UserId)
                        }.Save(Notify: true);

                        Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
                    }
                }

                Master.Feedback = "An unknown error ocurred";
            }
        }

        public void SubmitComplaint(object s, EventArgs e)
        {
            if (SelectedBooking == null || SelectedBooking.UserId != Master.CurrentUser.Id)
                Master.Feedback = "Booking not found";

            else if (SelectedBooking.StatusId == Core.Entities.BookingStatus.Complete)
                Master.Feedback = "This SelectedBooking has already been updated";

            else
            {
                SelectedBooking.StatusId = Core.Entities.BookingStatus.Incomplete;
                if (SelectedBooking.Save())
                {
                    new Core.Entities.Notification
                    {
                        Booking = SelectedBooking,
                        GroupId = Core.Entities.NotificationGroup.Offering,
                        Message = Complaint.Text,
                        TypeId = Core.Entities.NotificationType.ServiceIncomplete,
                        User = Core.Entities.User.Find(Id: SelectedBooking.Listing.UserId)
                    }.Save(Notify: true);

                    new Core.Entities.Notification
                    {
                        Booking = SelectedBooking,
                        GroupId = Core.Entities.NotificationGroup.Seeking,
                        Message = Complaint.Text,
                        TypeId = Core.Entities.NotificationType.ServiceIncomplete,
                        User = Core.Entities.User.Find(Id: SelectedBooking.UserId)
                    }.Save(Notify: true);

                    Core.Entities.Counter.Hit(UserId: SelectedBooking.UserId, Event: Core.Entities.Counters.IncompleteBooking, CategoryId: SelectedBooking.Service.CategoryId, BookingId: SelectedBooking.Id);

                    Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
                }

                Master.Feedback = "An unknown error occurred";
            }
        }

        public void InitiatePayment(object s, EventArgs e)
        {
            if (SelectedBooking == null || SelectedBooking.UserId != Master.CurrentUser.Id)
            {
                Master.Feedback = "Booking not found";
                return;
            }

            var users = new List<BookingUserModel>();

            if (!PayerMobileNumber.Text.IsEmpty())
            {
                users.Add(new BookingUserModel
                {
                    Name = PayerName.Text,
                    Telephone = PayerMobileNumber.Text,
                    Quantity = SelectedBooking.Quantity
                });
            }
            else
            {
                try
                {
                    users = JsonConvert.DeserializeObject<List<BookingUserModel>>(BookingUsers.Text);
                }
                catch { }
            }

            if (users.Count == 0)
            {
                Master.Feedback = "No payment details were submitted - please try again";
                return;
            }

            SelectedBooking.BookingUsers = Core.Entities.BookingUser.List(BookingId: SelectedBooking.Id);

            var SelectedBookingUsers = new List<Core.Entities.BookingUser>();
            foreach (var user in users)
            {
                if (user.Quantity == 0)
                    user.Quantity = 1;
                if (SelectedBooking.Quantity == 0)
                    SelectedBooking.Quantity = 1;

                var SelectedBookingUser = SelectedBooking.BookingUsers.FirstOrDefault(o => o.Telephone == user.Telephone);
                if (SelectedBookingUser == null)
                {
                    SelectedBookingUser = new Core.Entities.BookingUser
                    {
                        Booking = SelectedBooking,
                        Name = user.Name,
                        Ratio = user.Quantity / SelectedBooking.Quantity,
                        StatusId = Core.Entities.BookingUserStatus.Pending,
                        Telephone = user.Telephone
                    };

                    var registeredUser = Core.Entities.User.Find(Telephone: user.Telephone);
                    if (registeredUser.Id > 0)
                        SelectedBookingUser.User = registeredUser;

                    SelectedBookingUser.Save();
                    SelectedBooking.BookingUsers.Add(SelectedBookingUser);
                }
                else
                {
                    SelectedBookingUser.Ratio = user.Quantity / SelectedBooking.Quantity;

                    var registeredUser = Core.Entities.User.Find(Telephone: user.Telephone);
                    if (registeredUser.Id > 0)
                        SelectedBookingUser.User = registeredUser;

                    SelectedBookingUser.Save();
                }

                SelectedBookingUsers.Add(SelectedBookingUser);
            }

            foreach (var SelectedBookingUser in SelectedBooking.BookingUsers)
                if (SelectedBookingUsers.Count(o => o.Id == SelectedBookingUser.Id) == 0)
                    SelectedBookingUser.Delete();

            var success = true;
            var errorMessage = string.Empty;
            var transactions = Core.Entities.Transaction.List(BookingId: SelectedBooking.Id);
            foreach (var transaction in transactions.Where(o => o.StatusId == Core.Entities.TransactionStatus.Failed))
            {
                transaction.StatusId = Core.Entities.TransactionStatus.Error;
                transaction.Save();
            }

            foreach (var SelectedBookingUser in SelectedBookingUsers)
            {
                var transaction = transactions.FirstOrDefault(o =>
                    o.BookingUserId == SelectedBookingUser.Id &&
                    o.StatusId != Core.Entities.TransactionStatus.Error &&
                    o.StatusId != Core.Entities.TransactionStatus.Failed);

                if (transaction == null)
                {
                    transaction = new Core.Entities.Transaction
                    {
                        Amount = SelectedBooking.Price * SelectedBookingUser.Ratio,
                        Booking = SelectedBooking,
                        BookingUser = SelectedBookingUser,
                        StatusId = Core.Entities.TransactionStatus.Pending
                    };

                    transaction.Save();
                    transaction.RequestEcoCashPayment();
                    transactions.Add(transaction);

                    Core.Entities.Counter.Hit(UserId: SelectedBookingUser.UserId ?? 0, Event: Core.Entities.Counters.InitiatePayment, CategoryId: SelectedBooking.Service.CategoryId, BookingId: SelectedBooking.Id);

                    if (transaction.StatusId != Core.Entities.TransactionStatus.PendingSubscriberValidation)
                    {
                        success = false;
                        errorMessage = $"{SelectedBookingUser.Telephone}: {transaction.Error}";
                        SelectedBookingUser.Delete();
                    }
                }
            }

            if (success && transactions.Count > 0)
                Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
            else
                Master.Feedback = errorMessage.Coalesce("No transactions were created");
        }

        public void SavePop(object s, EventArgs e)
        {
            Master.Feedback = "Please upload your proof of payment";
            if (PopUpload.Photos.Count == 1)
            {
                SelectedBooking.ReceiptPhoto = PopUpload.Photos.First();
                if (SelectedBooking.PopReceived())
                    Master.Feedback = "Thank you for the proof of payment. The AgriShare team have been notified and will verify payment.";
            }
            Response.Redirect($"/account/booking/details?id={SelectedBooking.Id}");
        }
    }

    public class BookingUserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public decimal Quantity { get; set; }
    }
}