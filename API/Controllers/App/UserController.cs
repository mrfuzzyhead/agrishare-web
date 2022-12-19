using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class UserController : BaseApiController
    {
        [Route("register/telephone/lookup")]
        [AcceptVerbs("GET")]
        public object FindTelephone(string Telephone)
        {
            var user = Entities.User.Find(Telephone: Telephone);            
            if (user?.Id > 0 && !user.Telephone.IsEmpty())
                return Success(new
                {
                    User = new
                    {
                        user.Id,
                        user.FirstName,
                        user.StatusId
                    }
                });

            return Error("Please register to continue");
        }

        [Route("register")]
        [AcceptVerbs("POST")]
        public object Register(UserModel User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var user = new User
            {
                FirstName = User.FirstName,
                LastName = User.LastName,
                Telephone = User.Telephone,
                EmailAddress = User.EmailAddress,
                DateOfBirth = User.DateOfBirth,
                GenderId = User.GenderId,
                ClearPassword = User.PIN,
                LanguageId = User.LanguageId ?? Language.English,
                AgentName = User.AgentName
            };

            if (!string.IsNullOrEmpty(User.ReferralCode))
            {
                User referredBy = Entities.User.Find(ReferralCode: User.ReferralCode);
                if (referredBy != null && referredBy.Id > 0)
                    user.ReferredById = referredBy.Id;
                else
                    return Error("The referral code you entered was not recognised");
            }

            try
            {
                user.Region = Region.Find(User.RegionId.Value);
            }
            catch
            {
                user.Region = Region.Find((int)Regions.Zimbabwe);
            }

            if (user.LastName.Trim() == "asdaasd")
                return Error("An unknown error hasdaasd occurred!!");

            if (!Regex.IsMatch(user.Telephone, @"^0[\d]{9}"))
                return Error($"{user.Telephone} is not a valid cell number. The number should start with 0 and contain 10 digits.");

            if (!user.UniqueTelephone())
                return Error($"{user.Telephone} has already been registered");

            if (!user.EmailAddress.IsEmpty() && !user.UniqueEmailAddress())
                return Error($"{user.EmailAddress} has already been registered");

            user.RoleList = $"{Role.User}";
            user.NotificationPreferences = (int)Entities.NotificationPreferences.PushNotifications + (int)Entities.NotificationPreferences.SMS + (int)Entities.NotificationPreferences.BulkSMS;
            user.StatusId = UserStatus.Pending;
            if (user.AuthToken.IsEmpty())
                user.AuthToken = Guid.NewGuid().ToString();

            if (user.Save())
            {
                Counter.Hit(CurrentUser.Id, Counters.Register);

                return Success(new
                {
                    User = user.ProfileJson()
                });
            }

            return Error("An unknown error occurred.");
        }

        [Route("register/code/verify")]
        [AcceptVerbs("GET")]
        public object VerifyCode(int UserId, string Code)
        {
            var user = Entities.User.Find(Id: UserId);

            if (user?.FailedLoginAttempts > Entities.User.MaxFailedLoginAttempts)
                return Error("Your account has been locked - please reset your PIN.");

            if (user?.VerificationCode == Code)
            {
                if (user.VerificationCodeExpiry < DateTime.UtcNow)
                    return Error("This code has expired");

                user.VerificationCode = string.Empty;
                user.StatusId = Entities.UserStatus.Verified;
                user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                if (user.ReferredById.HasValue)
                    Entities.User.UpdateReferralCount(user.ReferredById.Value);

                return new
                {
                    User = user.ProfileJson()
                };
            }

            if (user.Id > 0)
            {
                user.FailedLoginAttempts += 1;
                user.Save();
            }

            return Error("Invalid code");
        }

        [Route("login")]
        [AcceptVerbs("GET")]
        public object Login(string Telephone, string PIN)
        {
            var user = Entities.User.Find(Telephone: Telephone);

            if (user == null || user?.Id == 0)
                return Error("Phone number or PIN not recognised.");

            if (user?.StatusId == Entities.UserStatus.Pending)
                return Error("Your account has not been verified - please reset your PIN.");

            if (user.FailedLoginAttempts > Entities.User.MaxFailedLoginAttempts)
                return Error("Your account has been locked - please reset your PIN.");

            if (user.ValidatePassword(PIN))
            {
                user.FailedLoginAttempts = 0;
                if (user.AuthToken.IsEmpty())
                    user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.Login);

                return Success(new
                {
                    Auth = new
                    {
                        User = user.ProfileJson()
                    }
                });
            }

            if (user.Id > 0)
            {
                user.FailedLoginAttempts += 1;
                user.Save();
            }

            return Error("Phone number or PIN not recognised.");
        }

        [Route("pin/change")]
        [AcceptVerbs("GET")]
        public object ChangePin(string Telephone, string Code, string PIN)
        {
            var user = Entities.User.Find(Telephone: Telephone);

            if (user == null || user?.Id == 0)
                return Error("Invalid code");

            if (user.VerificationCode != Code)
                return Error("Invalid code");

            user.ClearPassword = PIN;
            if (user.Save())
            {
                user.FailedLoginAttempts = 0;
                if (user.AuthToken.IsEmpty())
                    user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.ResetPIN);

                return Success(new
                {
                    User = user.ProfileJson()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("code/resend")]
        [AcceptVerbs("GET")]
        public object SendCode(string Telephone)
        {
            var user = Entities.User.Find(Telephone: Telephone);
            if (user?.Id > 0 && user.SendVerificationCode())
            {
                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.ForgotPIN);
                return Success("Please check your messages");
            }

            return Error("Unable to send verification code - please try again");
        }

        [@Authorize(Roles = "User")]
        [Route("code/request")]
        [AcceptVerbs("GET")]
        public object RequestCode()
        {
            var currentUser = Entities.User.Find(CurrentUser.Id);

            if (currentUser.StatusId == UserStatus.Verified)
                return Success(new
                {
                    User = currentUser.ProfileJson()
                });

            if (currentUser.OtpRequests > Entities.User.MaxOtpRequests)
            {
                SendVerificationRequest();
                return Error("A message has been sent to the Agrishare admin team. They will contact you to verify your account.");
            }

            if (currentUser.SendVerificationCode())
            {
                Counter.Hit(currentUser.Id, Counters.RequestOTP);
                return Success("Please check your messages");
            }

            return Error("Unable to send verification code - please try again");
        }

        [@Authorize(Roles = "User")]
        [Route("code/manual")]
        [AcceptVerbs("GET")]
        public object RequestVerification()
        {
            SendVerificationRequest();
            return Success("A message has been sent to the Agrishare admin team. They will contact you to verify your account.");
        }

        private void SendVerificationRequest()
        {
            var template = Template.Find(Title: "Verify Account");
            template.Replace("Name", CurrentUser.FullName);
            template.Replace("Telephone", CurrentUser.Telephone);
            template.Replace("Link", $"{Config.CMSURL}/#/users/detail/{CurrentUser.Id}");

            new Email
            {
                Message = template.EmailHtml(),
                RecipientEmail = Config.ApplicationEmailAddress,
                SenderEmail = CurrentUser.EmailAddress.Coalesce($"{CurrentUser.Telephone}@hariplay.app"),
                Subject = $"URGENT: {CurrentUser.FullName} - unable to verify account"
            }
            .Send();
        }

        [@Authorize(Roles = "User")]
        [Route("code/verify")]
        [AcceptVerbs("GET")]
        public object VerifyCode(string Code)
        {
            var currentUser = Entities.User.Find(CurrentUser.Id);

            if (currentUser?.FailedOtpAttempts > Entities.User.MaxFailedOtpAttempts)
                return Error("Your account has been locked - please reset your PIN.");

            if (currentUser?.VerificationCode == Code)
            {
                if (currentUser.VerificationCodeExpiry < DateTime.UtcNow)
                    return Error("This code has expired");

                currentUser.OtpRequests = 0;
                currentUser.VerificationCode = string.Empty;
                currentUser.StatusId = UserStatus.Verified;
                currentUser.AuthToken = Guid.NewGuid().ToString();
                currentUser.Save();

                if (currentUser.ReferredById.HasValue)
                    Entities.User.UpdateReferralCount(currentUser.ReferredById.Value);

                return new
                {
                    User = currentUser.ProfileJson()
                };
            }

            if (currentUser.Id > 0)
            {
                currentUser.FailedOtpAttempts += 1;
                currentUser.Save();
            }

            return Error("Invalid code");
        }

        [@Authorize(Roles = "User")]
        [Route("profile/update")]
        [AcceptVerbs("POST")]
        public object UpdateProfile(UserModel User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var currentUser = Entities.User.Find(CurrentUser.Id);

            var newTelephone = currentUser.Telephone != User.Telephone;

            currentUser.FirstName = User.FirstName;
            currentUser.LastName = User.LastName;
            currentUser.Telephone = User.Telephone;
            currentUser.EmailAddress = User.EmailAddress;
            currentUser.DateOfBirth = User.DateOfBirth;
            currentUser.GenderId = User.GenderId;
            currentUser.LanguageId = User.LanguageId ?? Entities.Language.English;

            try
            {
                currentUser.Region = Region.Find(User.RegionId.Value);
            }
            catch
            {
                currentUser.Region = Region.Find((int)Regions.Zimbabwe);
            }

            if (newTelephone)
                currentUser.StatusId = Entities.UserStatus.Pending;

            if (!Regex.IsMatch(currentUser.Telephone, @"^0[\d]{9}"))
                return Error($"{currentUser.Telephone} is not a valid cell number. The number should start with 0 and contain 10 digits.");

            if (!currentUser.UniqueTelephone())
                return Error($"{currentUser.Telephone} has already been registered");

            if (!currentUser.EmailAddress.IsEmpty() && !CurrentUser.UniqueEmailAddress())
                return Error($"{currentUser.EmailAddress} has already been registered");

            if (currentUser.Save())
            { 
                if (newTelephone)
                    currentUser.SendVerificationCode();

                return Success(new
                {
                    User = currentUser.ProfileJson(),
                    Verify = newTelephone
                });
            }

            return Error("An unknown error occurred");
        }

        [@Authorize(Roles = "User")]
        [Route("profile/preferences/notifications/update")]
        [AcceptVerbs("GET")]
        public object UpdateNotificationPreferences(bool SMS, bool PushNotifications, bool Email, bool BulkSMS = false)
        {
            var currentUser = Entities.User.Find(CurrentUser.Id);

            currentUser.NotificationPreferences = 0;
            if (SMS)
                currentUser.NotificationPreferences += (int)Entities.NotificationPreferences.SMS;
            if (PushNotifications)
                currentUser.NotificationPreferences += (int)Entities.NotificationPreferences.PushNotifications;
            if (Email)
                currentUser.NotificationPreferences += (int)Entities.NotificationPreferences.Email;
            if (BulkSMS)
                currentUser.NotificationPreferences += (int)Entities.NotificationPreferences.BulkSMS;

            if (currentUser.Save())
                return Success(new
                {
                    User = currentUser.ProfileJson()
                });

            return Error("Could not update preferences");
        }
               
        [@Authorize(Roles = "User")]
        [Route("profile/preferences/language/update")]
        [AcceptVerbs("GET")]
        public object UpdateLanguagePreference(Language LanguageId)
        {
            var currentUser = Entities.User.Find(CurrentUser.Id);

            currentUser.LanguageId = LanguageId;
            if (currentUser.Save())
                return Success(new
                {
                    User = CurrentUser.ProfileJson()
                });

            return Error("Could not update preferences");
        }

        [@Authorize(Roles = "User")]
        [Route("profile/preferences/payments/update")]
        [AcceptVerbs("GET")]
        public object UpdatePaymentPreferences([FromUri] PaymentPreferencesModel Preferences)
        {
            var currentUser = Entities.User.Find(CurrentUser.Id);

            currentUser.PaymentMethods = new List<PaymentMethod>();
            if (Preferences.Cash)
                currentUser.PaymentMethods.Add(PaymentMethod.Cash);
            if (Preferences.BankTransfer)
                currentUser.PaymentMethods.Add(PaymentMethod.BankTransfer);
            if (Preferences.MobileMoney)
                currentUser.PaymentMethods.Add(PaymentMethod.MobileMoney);
            currentUser.BankAccount.Bank = Preferences.BankName;
            currentUser.BankAccount.Branch = Preferences.BranchName;
            currentUser.BankAccount.AccountName = Preferences.AccountName;
            currentUser.BankAccount.AccountNumber = Preferences.AccountNumber;

            if (currentUser.Save())
                return Success(new
                {
                    User = currentUser.ProfileJson()
                });

            return Error("Could not update preferences");
        }

        [@Authorize(Roles = "User")]
        [Route("me")]
        [AcceptVerbs("GET")]
        public object Me()
        {
            return Success(new
            {
                User = CurrentUser.ProfileJson()
            });
        }

        [@Authorize(Roles = "User")]
        [Route("device/register")]
        [AcceptVerbs("GET")]
        public object RegisterDevice(string Token)
        {
            var device = Entities.Device.Find(Token: Token);

            if (device == null)
                device = new Entities.Device
                {
                    Token = Token,
                    User = CurrentUser
                };

            if (device.Save())
                return Success();

            return Error("Could not register device");
        }

        [Route("logout")]
        [AcceptVerbs("GET")]
        public object Logout(string Token)
        {
            var device = Entities.Device.Find(Token: Token);
            if (device?.Delete() ?? false)
                return Success();

            return Error("Device not found");
        }

        [Route("delete")]
        [AcceptVerbs("GET")]
        public object Delete()
        {
            var bookings = Entities.Booking.Count(UserId: CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
                return Error("Can not delete account - you have upcoming bookings");

            var listings = Entities.Listing.List(UserId: CurrentUser.Id);
            foreach(var listing in listings)
            {
                bookings = Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                    return Error("Can not delete account - you have upcoming bookings");
            }
            foreach (var listing in listings)
                listing.Delete();

            var devices = Entities.Device.List(UserId: CurrentUser.Id);
            foreach(var device in devices)
                device.Delete();            

            CurrentUser.Delete();

            return Success("OK");
        }
    }
}
