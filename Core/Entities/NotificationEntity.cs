using Agrishare.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Notification : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Title => Type;
        public string Type => $"{TypeId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public string Group => $"{GroupId}".ExplodeCamelCase();

        public static Notification Find(int Id = 0)
        {
            if (Id == 0)
                return new Notification
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Notification> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int UserId = 0, NotificationGroup GroupId = NotificationGroup.None, int BookingId = 0, NotificationType Type = NotificationType.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted && !o.Booking.Deleted && !o.Booking.Service.Deleted && !o.Booking.Service.Listing.Deleted);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (GroupId != NotificationGroup.None)
                    query = query.Where(o => o.GroupId == GroupId);

                if (BookingId > 0 && Type != NotificationType.None)
                    query = query.Where(e => e.BookingId == BookingId && e.TypeId == Type);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int UserId = 0, NotificationGroup GroupId = NotificationGroup.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted && !o.Booking.Deleted && !o.Booking.Service.Deleted && !o.Booking.Service.Listing.Deleted);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (GroupId != NotificationGroup.None)
                    query = query.Where(o => o.GroupId == GroupId);

                return query.Count();
            }
        }

        public bool Save(bool Notify = false)
        {
            var success = false;

            var user = User;
            if (user != null) UserId = user.Id;
            User = null;

            var booking = Booking;
            if (booking != null) BookingId = booking.Id;
            Booking = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;
            Booking = booking;

            if (Notify)
            {
                SendEmail(Config.ApplicationName, Config.ApplicationEmailAddress);

                if ((User.NotificationPreferences & (int)NotificationPreferences.Email) > 0)
                    SendEmail();

                if ((User.NotificationPreferences & (int)NotificationPreferences.PushNotifications) > 0)
                {
                    var devices = Device.List(UserId: UserId);
                    foreach (var device in devices)
                        SendPushNotification(device.EndpointARN);
                }

                if ((User.NotificationPreferences & (int)NotificationPreferences.SMS) > 0)
                    SendSMS();
            }

            return success;
        }

        private bool Add()
        {
            StatusId = NotificationStatus.Pending;

            using (var ctx = new AgrishareEntities())
            {
                ctx.Notifications.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Notifications.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                User = User?.Json(),
                Booking = Booking?.Json(),
                Title,
                TypeId,
                Type,
                StatusId,
                Status,
                GroupId,
                Group,
                DateCreated
            };
        }

        private void SendEmail(string RecipientName = "", string RecipientEmailAddress = "")
        {
            var template = Template.Find(Title: Type);

            if (Booking.Service != null)
                template.Replace("Service", Booking.Service.Category?.Title);
            else
                template.ReplaceSectionTemplate("Service", string.Empty);

            if (Booking.Supplier != null)
                template.Replace("Supplier", Booking.Supplier.Title);
            else
                template.ReplaceSectionTemplate("Supplier", string.Empty);

            if (Booking.Products.Count > 0)
            {
                var productsHtml = new StringBuilder();
                var productRowTemplate = template.GetSectionTemplate("Product Row");
                foreach(var product in Booking.Products)
                {
                    var html = productRowTemplate;
                    html = Template.Replace(html, "Product Title", product.Title);
                    productsHtml.AppendLine(html);
                }
                template.ReplaceSectionTemplate("Product Row", productsHtml.ToString());
            }
            else
                template.ReplaceSectionTemplate("Product Row", string.Empty);

            template.Replace("Message", Message);
            template.Replace("Start Date", Booking.StartDate.ToString("d MMMM yyyy"));
            template.Replace("End Date", Booking.EndDate.ToString("d MMMM yyyy"));

            template.Replace("Payment Amount", Template.FormatCurrency(Booking.Price));
            template.Replace("Payment Fees", Template.FormatCurrency(Booking.AgriShareCommission));
            template.Replace("Payment Total", Template.FormatCurrency(Booking.Price - Booking.AgriShareCommission));

            var subject = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    subject = $"Booking cancelled";
                    break;
                case NotificationType.BookingConfirmed:
                    subject = $"Booking confirmed by supplier";
                    break;
                case NotificationType.NewBooking:
                    subject = $"New booking received";
                    break;
                case NotificationType.NewReview:
                    subject = $"New review received";
                    break;
                case NotificationType.PaymentReceived:
                    subject = $"Payment received";
                    break;
                case NotificationType.ServiceComplete:
                    subject = $"Service complete";
                    break;
                case NotificationType.ServiceIncomplete:
                    subject = $"Service not complete";
                    break;
            }

            subject = $"{subject} (#{Id})";

            new Email
            {
                Message = template.EmailHtml(),
                RecipientEmail = RecipientEmailAddress.Coalesce(User.EmailAddress),
                RecipientName = RecipientName.Coalesce(User.FullName),
                SenderEmail = Config.ApplicationEmailAddress,
                Subject = subject
            }.Send();
        }

        private void SendSMS()
        {
            var message = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    message = string.Format(Translations.Translate(TranslationKey.BookingCancelled, User.LanguageId), BookingId);
                    break;
                case NotificationType.BookingConfirmed:
                    message = string.Format(Translations.Translate(TranslationKey.BookingConfirmed, User.LanguageId), BookingId);
                    break;
                case NotificationType.NewBooking:
                    message = string.Format(Translations.Translate(TranslationKey.BookingReceived, User.LanguageId), BookingId);
                    break;
                case NotificationType.NewReview:
                    message = string.Format(Translations.Translate(TranslationKey.NewReview, User.LanguageId), BookingId);
                    break;
                case NotificationType.PaymentReceived:
                    message = string.Format(Translations.Translate(TranslationKey.PaymentReceived, User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceComplete:
                    message = string.Format(Translations.Translate(TranslationKey.BookingCompleted, User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceIncomplete:
                    message = string.Format(Translations.Translate(TranslationKey.BookingNotCompleted, User.LanguageId), BookingId);
                    break;
            }

            if (SMS.SendMessage(User.Telephone, message))
            {
                var booking = Booking.Find(Booking?.Id ?? BookingId ?? 0);
                if (booking.Id > 0)
                {
                    booking.SMSCount += 1;
                    booking.SMSCost += Transaction.SMSCost;
                    booking.Save();
                }
            }
        }

        private void SendPushNotification(string DeviceARN)
        {
            var message = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    message = string.Format(Translations.Translate(TranslationKey.BookingCancelled, User.LanguageId), BookingId);
                    break;
                case NotificationType.BookingConfirmed:
                    message = string.Format(Translations.Translate(TranslationKey.BookingConfirmed, User.LanguageId), BookingId);
                    break;
                case NotificationType.NewBooking:
                    message = string.Format(Translations.Translate(TranslationKey.BookingReceived, User.LanguageId), BookingId);
                    break;
                case NotificationType.NewReview:
                    message = string.Format(Translations.Translate(TranslationKey.NewReview, User.LanguageId), BookingId);
                    break;
                case NotificationType.PaymentReceived:
                    message = string.Format(Translations.Translate(TranslationKey.PaymentReceived, User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceComplete:
                    message = string.Format(Translations.Translate(TranslationKey.BookingCompleted, User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceIncomplete:
                    message = string.Format(Translations.Translate(TranslationKey.BookingNotCompleted, User.LanguageId), BookingId);
                    break;
            }

            var args = new Dictionary<string, object>
            {
                { "UserId", UserId },
                { "BookingId", BookingId }
            };
            SNS.SendMessage(DeviceARN, message, $"app.agrishare.category.{TypeId}", args);
        }

        public object AppDashboardJson()
        {
            var service = Booking.Service?.Category?.Title ?? Booking.Supplier?.Title ?? "";
            var photoUrl = $"{Config.CDNURL}/" + (Booking.Listing?.Photos.FirstOrDefault()?.ThumbName ?? Booking.Products?.FirstOrDefault()?.Photo?.ThumbName ?? "NoImage.png");

            return new
            {
                Id,
                Title,
                TypeId,
                StatusId,
                Booking.StartDate,
                Booking.EndDate,
                DateCreated,
                Service = service,
                PhotoUrl = photoUrl,
                BookingId = Booking.Id,
                BookingStatusId = Booking.StatusId
            };
        }
    }
}
