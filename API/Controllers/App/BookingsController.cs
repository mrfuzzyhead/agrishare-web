﻿using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class BookingsController : BaseApiController
    {
        [Route("bookings/add")]
        [AcceptVerbs("POST")]
        public object AddBooking(BookingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var service = Entities.Service.Find(Id: Model.ServiceId);
            if (service == null)
                return Error("Invalid service selected");

            var booking = new Entities.Booking
            {
                DestinationLatitude = Model.DestinationLatitude,
                Destination = Model.Destination,
                DestinationLongitude = Model.DestinationLongitude,
                ForId = Model.ForId,
                IncludeFuel = Model.IncludeFuel,
                Latitude = Model.Latitude,
                Location = Model.Location,
                Longitude = Model.Longitude,
                Quantity = Model.Quantity,
                Service = service,
                Listing = service.Listing,
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                StatusId = Entities.BookingStatus.Pending,
                User = CurrentUser,
                AdditionalInformation = Model.AdditionalInformation,
                TotalVolume = Model.TotalVolume,
                HireCost = Model.HireCost,
                FuelCost = Model.FuelCost,
                TransportCost = Model.TransportCost,
                Price = Model.HireCost + Model.FuelCost + Model.TransportCost,
                Distance = Model.Distance,
                TransportDistance = Model.TransportDistance,
                Commission = Entities.Transaction.AgriShareCommission,
                AgentCommission = CurrentUser.Agent?.Commission ?? 0,
                IMTT = Entities.Transaction.IMTT
            };       

            if (booking.Save())
            {
                Entities.Counter.Hit(UserId: CurrentUser.Id, Event: Entities.Counters.Book, CategoryId: booking.Service.CategoryId, BookingId: booking.Id);

                var users = booking.ProviderUsers();
                foreach(var user in users)
                    new Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Entities.NotificationGroup.Offering,
                        TypeId = Entities.NotificationType.NewBooking,
                        User = user
                    }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.NewBooking,
                    User = CurrentUser
                }.Save(Notify: false);

                return Success(new
                {
                    Booking = booking.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/confirm")]
        [AcceptVerbs("GET")]
        public object ConfirmBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || !booking.IsProvider(CurrentUser))
                return Error("Booking not found");

            if (booking.StatusId != Entities.BookingStatus.Pending)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Approved;
            if (booking.Save())
            {
                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.NewBooking);
                foreach(var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingConfirmed,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.ConfirmBooking, CategoryId: booking.Service?.CategoryId, BookingId: booking.Id);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/decline")]
        [AcceptVerbs("GET")]
        public object DeclineBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || !booking.IsProvider(CurrentUser))
                return Error("Booking not found");

            if (booking.StatusId != Entities.BookingStatus.Pending)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Declined;
            if (booking.Save())
            {
                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.NewBooking);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.DeclineBooking, CategoryId: booking.Service?.CategoryId, BookingId: booking.Id);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/complete")]
        [AcceptVerbs("GET")]
        public object CompleteBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || !booking.IsOwner(CurrentUser))
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Complete;
            if (booking.Save())
            {
                var users = booking.ProviderUsers();
                foreach(var user in users)
                    new Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Entities.NotificationGroup.Offering,
                        TypeId = Entities.NotificationType.ServiceComplete,
                        User = user
                    }.Save(Notify: true);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.CompleteBooking, CategoryId: booking.Service?.CategoryId, BookingId: booking.Id);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/incomplete")]
        [AcceptVerbs("GET")]
        public object IncompleteBooking(int BookingId, string Message)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || !booking.IsOwner(CurrentUser))
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Incomplete;
            if (booking.Save())
            {
                var users = booking.ProviderUsers();
                foreach(var user in users)
                    new Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Entities.NotificationGroup.Offering,
                        Message = Message,
                        TypeId = Entities.NotificationType.ServiceIncomplete,
                        User = user
                    }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    Message = Message,
                    TypeId = Entities.NotificationType.ServiceIncomplete,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.IncompleteBooking, CategoryId: booking.Service?.CategoryId, BookingId: booking.Id);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/seeking")]
        [AcceptVerbs("GET")]
        public object SeekingList(int PageIndex = 0, int PageSize = 25)
        {
            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var monthlySpend = Entities.Booking.SeekingSummary(CurrentUser.Id, startDate);
            var totalSpend = Entities.Booking.SeekingSummary(CurrentUser.Id);
            var commissionEarned = 0M;
            if (CurrentUser.Agent != null)
                commissionEarned = Entities.Booking.SeekingSummaryAgentCommission(CurrentUser.Id);

            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id);

            return Success(new
            {
                Bookings = bookings.Select(e => e.AppDashboardJson()),
                Summary = new
                {
                    Month = monthlySpend,
                    Total = totalSpend,
                    Commission = commissionEarned
                }
            });
        }

        [Route("bookings/offering")]
        [AcceptVerbs("GET")]
        public object OfferingList(int PageIndex = 0, int PageSize = 25)
        {
            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var monthlySpend = Entities.Booking.OfferingSummary(UserId: CurrentUser.Id, SupplierId: CurrentUser.Supplier?.Id ?? 0, StartDate: startDate);
            var totalSpend = Entities.Booking.OfferingSummary(UserId: CurrentUser.Id, SupplierId: CurrentUser.Supplier?.Id ?? 0);
            
            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, ListingUserId: CurrentUser.Id, ListingSupplierId: CurrentUser.Supplier?.Id ?? 0);

            return Success(new
            {
                Bookings = bookings.Select(e => e.AppDashboardJson()),
                Summary = new
                {
                    Month = monthlySpend,
                    MonthCommission = monthlySpend * Entities.Transaction.AgriShareCommission,
                    Total = totalSpend,
                    TotalCommission = totalSpend * Entities.Transaction.AgriShareCommission
                }
            });
        }

        [Route("bookings/listing")]
        [AcceptVerbs("GET")]
        public object ListingBookings(int ListingId, int PageIndex = 0, int PageSize = 25)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, ListingId: ListingId);

            return Success(new
            {
                Bookings = bookings.Select(e => e.AppDashboardJson())
            });
        }

        [Route("bookings/detail")]
        [AcceptVerbs("GET")]
        public object BookingDetail(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || (!booking.IsOwner(CurrentUser) && !booking.IsProvider(CurrentUser)))
                return Error("Booking does not exist");

            var ratingCount = 0;
            if (booking.ListingId.HasValue)
                ratingCount = Entities.Rating.Count(ListingId: booking.ListingId.Value, UserId: CurrentUser.Id);

            var bookingUsers = Entities.BookingUser.List(BookingId: booking.Id);

            var isOwner = CurrentUser.Id == booking.Listing?.UserId || (booking.Supplier != null && CurrentUser.SupplierId == booking.Supplier?.Id);

            return Success(new
            {
                Booking = booking.Json(),
                Users = bookingUsers.Select(e => e.Json()),
                Rated = ratingCount > 0,
                Entities.Journal.CurrentRate,
                Entities.Config.AgriShareBankDetails,
                Entities.Config.AgriShareOfficeLocation,
                IsOwner = isOwner
            });
        }

        [Route("bookings/invoice/pdf")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage InvoicePDF(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || (!booking.IsOwner(CurrentUser) && !booking.IsProvider(CurrentUser)))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            var dataStream = new MemoryStream(booking.InvoicePDF());
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = $"INV-{booking.Id.ToString().PadLeft(8, '0')}.pdf"
            };
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            return httpResponseMessage;
        }

        [Route("bookings/users/verify")]
        [AcceptVerbs("GET")]
        public object VerifyUser(int BookingUserId, string VerificationCode)
        {
            var bookingUser = Entities.BookingUser.Find(Id: BookingUserId);
            if (bookingUser == null || bookingUser.Id == 0)
                return Error("User does not exist");

            var booking = Entities.Booking.Find(Id: bookingUser.BookingId);
            if (booking == null || booking.Id == 0 || booking.UserId != CurrentUser.Id)
                return Error("Booking does not exist");

            if (bookingUser.VerificationCode == VerificationCode)
            {
                bookingUser.StatusId = Entities.BookingUserStatus.Verified;
                bookingUser.Save();

                return Success(new
                {
                    BookingUser = bookingUser.Json()
                });
            }

            return Error("Invalid code");
        }

        [Route("bookings/cancel")]
        [AcceptVerbs("GET")]
        public object CancelBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || (!booking.IsOwner(CurrentUser) && !booking.IsProvider(CurrentUser)))
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("You can not cancel a completed booking");

            if (booking.StatusId == Entities.BookingStatus.InProgress)
                return Error("You can not cancel a paid booking that is not complete");

            if (DateTime.Now.StartOfDay() >= booking.StartDate.StartOfDay())
                return Error("You can not cancel a booking after the start date");

            booking.StatusId = Entities.BookingStatus.Cancelled;
            if (booking.Save())
            {
                var transactions = Entities.Transaction.List(BookingId: booking.Id);
                foreach(var transaction in transactions)
                    transaction.RequestEcoCashRefund();

                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.BookingCancelled);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                var users = booking.ProviderUsers();
                foreach(var user in users)
                    new Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Entities.NotificationGroup.Offering,
                        TypeId = Entities.NotificationType.BookingCancelled,
                        User = user
                    }.Save(Notify: false);

                Entities.Counter.Hit(UserId: booking.UserId, Event: Entities.Counters.CancelBooking, CategoryId: booking.Service?.CategoryId, BookingId: booking.Id);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/pop")]
        [AcceptVerbs("POST")]
        public object Add(PopModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var booking = Entities.Booking.Find(Model.BookingId);

            if (booking == null || !booking.IsOwner(CurrentUser))
                return Error("Booking not found");

            var photoFilename = Entities.File.SaveBase64Image(Model.Photo.Base64);
            var photo = new Entities.File(photoFilename);
            photo.Resize(200, 200, photo.ThumbName);
            photo.Resize(800, 800, photo.ZoomName);

            booking.ReceiptPhoto = photo;
            if (booking.PopReceived())
                return BookingDetail(Model.BookingId);

            return Error();
        }

        [Route("bookings/products/add")]
        [AcceptVerbs("POST")]
        public object AddProductBooking(ProductBookingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var productIds = Model.ProductIds.Split(',').Where(e => !string.IsNullOrEmpty(e)).Select(e => Convert.ToInt32(e)).ToList();

            // check availability
            var unavailableProducts = Entities.Product.Unavailable(productIds, Model.StartDate, Model.EndDate);
            if (unavailableProducts.Count > 0)
            {
                var errorMessage = "The following items are not available between the selected dates: " + string.Join(", ", unavailableProducts.Select(e => e.Title));
                return Error(errorMessage);
            }

            // get product list and supplier list           
            var supplierList = new List<Entities.Supplier>();
            var productList = new List<Entities.Product>();
            foreach (var id in productIds)
            {
                var product = Entities.Product.Find(id);
                productList.Add(Entities.Product.Find(id));
                if (supplierList.Count(e => e.Id == product.SupplierId) == 0)
                    supplierList.Add(product.Supplier);
            }

            // create one booking per supplier
            var bookingList = new List<Entities.Booking>();
            foreach (var supplier in supplierList)
            {                
                var supplierProductList = productList.Where(e => e.SupplierId == supplier.Id).ToList();
                var dayCount = (int)Math.Ceiling((Model.EndDate - Model.StartDate).TotalDays) + 1;
                var hireCost = supplierProductList.Sum(e => e.DayRate) * dayCount * (1 + Core.Entities.Transaction.AgriShareCommission);
                var transportDistance = (int)Math.Ceiling(Location.GetDistance(supplier.Longitude, supplier.Latitude, Model.Longitude, Model.Latitude) / 1000);
                var transportCost = transportDistance * 2 * supplier.TransportCostPerKm * dayCount;                

                var booking = new Entities.Booking
                {
                    ForId = Entities.BookingFor.Me,
                    IncludeFuel = true,
                    Latitude = Model.Latitude,
                    Location = Model.Location,
                    Longitude = Model.Longitude,
                    Quantity = 1,
                    StartDate = Model.StartDate,
                    EndDate = Model.EndDate,
                    StatusId = Entities.BookingStatus.Pending,
                    User = CurrentUser,
                    AdditionalInformation = Model.AdditionalInformation,
                    HireCost = hireCost,
                    TransportCost = transportCost,
                    Price = hireCost + transportCost,
                    Distance = transportDistance,
                    TransportDistance = transportDistance,
                    Commission = Entities.Transaction.AgriShareCommission,
                    AgentCommission = CurrentUser.Agent?.Commission ?? 0,
                    IMTT = Entities.Transaction.IMTT,
                    Products = supplierProductList,
                    Supplier = supplier
                };

                if (booking.Save())
                {
                    foreach (var product in booking.Products)
                        booking.AddProduct(product.Id);

                    var users = Entities.User.List(SupplierId: supplier.Id);
                    foreach(var user in users)
                        new Entities.Notification
                        {
                            Booking = booking,
                            GroupId = Entities.NotificationGroup.Offering,
                            TypeId = Entities.NotificationType.NewBooking,
                            User = user
                        }.Save(Notify: true);

                    new Entities.Notification
                    {
                        Booking = booking,
                        GroupId = Entities.NotificationGroup.Seeking,
                        TypeId = Entities.NotificationType.NewBooking,
                        User = CurrentUser
                    }.Save(Notify: false);

                    bookingList.Add(booking);
                }

                Entities.Counter.Hit(UserId: CurrentUser.Id, Event: Entities.Counters.Book, CategoryId: 0, BookingId: booking.Id);
            }

            if (bookingList.Count > 0)
                return Success(new
                {
                    Bookings = bookingList.Select(e => e.Json())
                });

            return Error("An unknown error occurred");
        }
    }
}
