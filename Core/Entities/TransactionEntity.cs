using Agrishare.Core.Utils;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
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
    public partial class Transaction : IEntity
    {
        public static bool LivePayments => Convert.ToBoolean(Config.Find(Key: "Live Payments")?.Value ?? "True");
        public static decimal AgriShareCommission => Convert.ToDecimal(Config.Find(Key: "AgriShare Commission").Value);
        public static decimal SMSCost => Convert.ToDecimal(Config.Find(Key: "SMS Cost").Value);
        public static decimal IMTT => Convert.ToDecimal(Config.Find(Key: "IMTT").Value);

        public static string DefaultSort = "DateCreated DESC";
        public string Title => "AGR-" + Id.ToString().PadLeft(8, '0');
        public string Status => $"{StatusId}".ExplodeCamelCase();

        public static Transaction Find(int Id = 0, string ClientCorrelator = "", string EcoCashReference = "")
        {
            if (Id == 0 && ClientCorrelator.IsEmpty() && EcoCashReference.IsEmpty())
                return new Transaction
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Include(o => o.BookingUser).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);
                if (!ClientCorrelator.IsEmpty())
                    query = query.Where(e => e.ClientCorrelator == ClientCorrelator);
                if (!EcoCashReference.IsEmpty())
                    query = query.Where(e => e.EcoCashReference == EcoCashReference);

                return query.FirstOrDefault();
            }
        }

        public static List<Transaction> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "",
            int BookingId = 0, int BookingUserId = 0, TransactionStatus StatusId = TransactionStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Include(o => o.BookingUser).Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (BookingUserId > 0)
                    query = query.Where(o => o.BookingUserId == BookingUserId);

                if (StatusId != TransactionStatus.None)
                    query = query.Where(o => o.StatusId == StatusId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int BookingId = 0, int BookingUserId = 0, TransactionStatus StatusId = TransactionStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (BookingUserId > 0)
                    query = query.Where(o => o.BookingUserId == BookingUserId);

                if (StatusId != TransactionStatus.None)
                    query = query.Where(o => o.StatusId == StatusId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var booking = Booking;
            if (booking != null) BookingId = booking.Id;
            Booking = null;

            var bookingUser = BookingUser;
            if (bookingUser != null) BookingUserId = bookingUser.Id;
            BookingUser = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;
            BookingUser = bookingUser;

            return success;
        }

        private bool Add()
        {
            //ClientCorrelator =  Guid.NewGuid().ToString();
            ClientCorrelator = GenerateRandomCode(16);

            using (var ctx = new AgrishareEntities())
            {
                ctx.Transactions.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Transactions.Attach(this);
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
                BookingId,
                BookingUser = BookingUser?.Json(),
                ServerReference,
                EcoCashReference,
                Amount,
                Currency,
                CurrencyCode = $"{Currency}",
                StatusId,
                Status,
                Error,
                Gateway,
                DateCreated
            };
        }

        private void SendPaymentNotification()
        {
            //TODO send payment notification
        }

        #region Generic payment methods - use Payment Gateway enum

        public void RequestStatus()
        {
            switch (Gateway)
            {
                case PaymentGateway.EcoCashZimbabwe:
                    RequestEcoCashStatus();
                    break;
                case PaymentGateway.AirtelUganda:
                    RequestAirtelStatus();
                    break;
                case PaymentGateway.MTNUganda:
                    RequestMtnStatus();
                    break;
                default:
                    RequestEcoCashStatus();
                    break;
            }
        }

        public bool RequestPayment()
        {
            switch (Gateway)
            {
                case PaymentGateway.EcoCashZimbabwe:
                    return RequestEcoCashPayment();
                case PaymentGateway.AirtelUganda:
                    return RequestAirtelPayment();
                case PaymentGateway.MTNUganda:
                    return RequestMtnPayment();
                default:
                    return RequestEcoCashPayment();
            }
        }

        public bool RequestRefund()
        {
            switch (Gateway)
            {
                case PaymentGateway.EcoCashZimbabwe:
                    return RequestEcoCashRefund();
                case PaymentGateway.AirtelUganda:
                    return RequestAirtelRefund();
                case PaymentGateway.MTNUganda:
                    return RequestMtnRefund();
                default:
                    return RequestEcoCashRefund();
            }
        }

        public void PaymentComplete()
        {
            BookingUser.StatusId = BookingUserStatus.Paid;
            BookingUser.Save();

            if (Booking == null)
                Booking = Booking.Find(BookingId);

            new Journal
            {
                Amount = Booking.Price,
                BookingId = BookingId,
                EcoCashReference = EcoCashReference,
                Reconciled = false,
                Title = $"Payment received from {BookingUser.Name} {BookingUser.Telephone}",
                TypeId = JournalType.Payment,
                UserId = BookingUser.UserId ?? Booking?.UserId,
                Date = DateTime.UtcNow,
                Currency = Entities.Currency.ZWL,
                CurrencyAmount = Amount,
                RegionId = Booking.Listing.RegionId
            }.Save();

            var bookingUsers = BookingUser.List(BookingId: BookingId);
            if (bookingUsers.Count(e => e.StatusId != BookingUserStatus.Paid) == 0)
            {
                var booking = Booking.Find(BookingId);
                booking.StatusId = BookingStatus.InProgress;

                var transactionFee = TransactionFee.Find(booking.Price - booking.AgriShareCommission);
                if (transactionFee != null)
                {
                    if (transactionFee.FeeType == FeeType.Fixed)
                        booking.TransactionFee = transactionFee.Fee;
                    else
                        booking.TransactionFee = (booking.Price - booking.AgriShareCommission) * transactionFee.Fee;
                }

                booking.IMTT = (booking.Price - booking.AgriShareCommission) * IMTT;
                booking.Save();

                var notifications = Notification.List(BookingId: booking.Id, Type: NotificationType.BookingConfirmed);
                foreach (var notification in notifications)
                {
                    notification.StatusId = NotificationStatus.Complete;
                    notification.Save();
                }

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Offering,
                    TypeId = NotificationType.PaymentReceived,
                    User = User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Seeking,
                    TypeId = NotificationType.PaymentReceived,
                    User = User.Find(Id: booking.UserId)
                }.Save(Notify: false);

                Counter.Hit(UserId: BookingUser.UserId ?? 0, Event: Counters.CompletePayment, CategoryId: booking.Service.CategoryId, BookingId: booking.Id);
                SendPaymentNotification();
            }
        }

        public void RefundComplete(int originalId)
        {
            StatusId = TransactionStatus.Completed;

            Amount *= -1;

            var originalTransaction = Find(Id: originalId);
            originalTransaction.StatusId = TransactionStatus.Refunded;
            originalTransaction.Save();

            new Journal
            {
                Amount = Amount,
                BookingId = BookingId,
                EcoCashReference = EcoCashReference,
                Reconciled = false,
                Title = $"Payment refunded to {BookingUser.Name} {BookingUser.Telephone}",
                TypeId = JournalType.Refund,
                UserId = BookingUser.Id,
                Date = DateTime.UtcNow
            }.Save();
        }

        #endregion

        #region MTN

        private static string MTNUrl => Config.Find(Key: "MTN URL").Value;
        private static string MTNSubscriptionKey => Config.Find(Key: "MTN Subscription Key").Value;
        private static string MTNUserId => Config.Find(Key: "MTN User ID").Value;
        private static string MTNApiKey => Config.Find(Key: "MTN API Key").Value;
        private static string MTNEnvironment => Config.Find(Key: "MTN Environment").Value;
        private static string MTNCurrency => Config.Find(Key: "MTN Currency").Value;
        private static bool MTNLog => Config.Find(Key: "MTN Log").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        private string _mtnAccessToken = "";
        private string MTNAccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_mtnAccessToken))
                {
                    try
                    {
                        var client = new RestClient($"{MTNUrl}/collection/token/");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddHeader("Cache-Control", "no-cache");

                        string svcCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(MTNUserId + ":" + MTNApiKey));
                        request.AddHeader("Authorization", $"Basic {svcCredentials}");
                        request.AddHeader("Ocp-Apim-Subscription-Key", MTNSubscriptionKey);

                        var response = client.Execute<dynamic>(request);

                        if (AirtelLog)
                            Entities.Log.Debug(
                                "Transaction.MTNAccessToken",
                                client.BaseUrl +
                                Environment.NewLine +
                                Environment.NewLine +
                                JsonConvert.SerializeObject(response) +
                                Environment.NewLine +
                                Environment.NewLine);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            _mtnAccessToken = response.Data["access_token"];
                    }
                    catch (Exception ex)
                    {
                        Entities.Log.Error("Transaction.MTNAccessToken", ex);
                    }
                }

                return _mtnAccessToken;
            }
        }

        public bool RequestMtnPayment()
        {
            Save();

            if (!LivePayments)
            {
                ServerReference = "DEMO-" + Guid.NewGuid().ToString();
                StatusId = TransactionStatus.PendingSubscriberValidation; // Updated when polled
                Save();

                return true;
            }

            try
            {
                ServerReference = Guid.NewGuid().ToString();

                var client = new RestClient($"{MTNUrl}/collection/v1_0/requesttopay");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Authorization", $"Bearer {MTNAccessToken}");
                request.AddHeader("Ocp-Apim-Subscription-Key", MTNSubscriptionKey);
                request.AddHeader("X-Reference-Id", ServerReference);
                request.AddHeader("X-Target-Environment", MTNEnvironment);
                // TODO on production
                request.AddHeader("X-Callback-Url", $"{Config.APIURL}/transactions/mtn/notify");
                var body = new
                {
                    amount = Amount.ToString("0.##"),
                    currency = MTNCurrency,
                    externalId = Id.ToString(),
                    payer = new
                    {
                        partyIdType = "MSISDN",
                        partyId = SanitiseUgMobileNumber(BookingUser.Telephone)
                    },
                    payerMessage = "Agrishare booking",
                    payerNote = Title
                };
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
                var response = client.Execute<dynamic>(request);

                if (AirtelLog)
                    Entities.Log.Debug(
                        "Transaction.RequestMtnPayment",
                        client.BaseUrl +
                        Environment.NewLine +
                        Environment.NewLine +
                        JsonConvert.SerializeObject(response) +
                        Environment.NewLine +
                        Environment.NewLine);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    StatusId = TransactionStatus.PendingSubscriberValidation;
                    Save();
                }
                else
                {
                    StatusId = TransactionStatus.Error;
                    Error = response.Data["message"].ToString();
                    Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Entities.Log.Error("Transaction.RequestMtnPayment", ex);
            }

            Error = "An unknown error occurred";
            StatusId = TransactionStatus.Error;
            Save();
            return false;
        }

        public bool RequestMtnRefund()
        {
            return false;
        }

        public void RequestMtnStatus()
        {
            var previousStatusId = StatusId;

            if (!LivePayments)
            {
                StatusId = TransactionStatus.Completed;
                EcoCashReference = "DEM-" + Guid.NewGuid().ToString();
            }
            else
            {
                try
                {
                    var client = new RestClient($"{MTNUrl}/collection/v1_0/requesttopay/{ServerReference}");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Cache-Control", "no-cache");
                    request.AddHeader("Authorization", $"Bearer {AirtelAccessToken}");
                    request.AddHeader("X-Target-Environment", MTNEnvironment);
                    request.AddHeader("Ocp-Apim-Subscription-Key", MTNSubscriptionKey);

                    var response = client.Execute<dynamic>(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var status = response.Data["status"];
                        var reason_message = response.Data["reason"]["message"];
                        var financial_transaction_id = response.Data["financial_transaction_id"];

                        if (status == "SUCCESSFUL")
                        {
                            StatusId = TransactionStatus.Completed;
                            EcoCashReference = financial_transaction_id;
                        }
                        else if (status == "FAILED")
                        {
                            Error = reason_message;
                            StatusId = TransactionStatus.Error;
                        }
                    }
                    else
                    {
                        Error = "An unknown error occurred";
                        StatusId = TransactionStatus.Error;
                    }
                }
                catch (Exception ex)
                {
                    Entities.Log.Error("Transaction.RequestMtnStatus", ex);
                }
            }

            Save();

            if (previousStatusId != TransactionStatus.Completed && StatusId == TransactionStatus.Completed)
                PaymentComplete();
        }

        #endregion

        #region airtel

        public static string SanitiseUgMobileNumber(string MobileNumber)
        {
            if (string.IsNullOrEmpty(MobileNumber))
                return string.Empty;

            MobileNumber = Regex.Replace(MobileNumber, " ", "").Trim();

            if (MobileNumber.StartsWith("0"))
                return Regex.Replace(MobileNumber, "^0", "");
            else if (MobileNumber.StartsWith("256"))
                return Regex.Replace(MobileNumber, "^256", "");

            return MobileNumber;
        }

        private static string AirtelUrl => Config.Find(Key: "Airtel URL").Value;
        private static string AirtelClientId => Config.Find(Key: "Airtel Client ID").Value;
        private static string AirtelClientSecret => Config.Find(Key: "Airtel Client Secret").Value;
        private static bool AirtelLog => Config.Find(Key: "Airtel Log").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        private string _airtelAccessToken = "";
        private string AirtelAccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_airtelAccessToken))
                {
                    try
                    {
                        var client = new RestClient($"{AirtelUrl}/auth/oauth2/token");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Cache-Control", "no-cache");
                        var body = new
                        {
                            client_id = AirtelClientId,
                            client_secret = AirtelClientSecret,
                            grant_type = "client_credentials"
                        };
                        request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
                        var response = client.Execute<dynamic>(request);

                        if (AirtelLog)
                            Entities.Log.Debug(
                                "Transaction.AirtelAccessToken",
                                client.BaseUrl +
                                Environment.NewLine +
                                Environment.NewLine +
                                JsonConvert.SerializeObject(response) +
                                Environment.NewLine +
                                Environment.NewLine);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            _airtelAccessToken = response.Data["access_token"];
                    }
                    catch (Exception ex)
                    {
                        Entities.Log.Error("Transaction.AirtelAccessToken", ex);
                    }
                }

                return _airtelAccessToken;
            }
        }

        public bool RequestAirtelPayment()
        {
            Save();

            if (!LivePayments)
            {
                ServerReference = "DEMO-" + Guid.NewGuid().ToString();
                StatusId = TransactionStatus.PendingSubscriberValidation; // Updated when polled
                Save();

                return true;
            }

            try
            {
                var client = new RestClient($"{AirtelUrl}/merchant/v1/payments/");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("X-Country", "UG");
                request.AddHeader("X-Currency", "UGX");
                request.AddHeader("Authorization", $"Bearer {AirtelAccessToken}");
                var body = new
                {
                    reference = "Agrishare booking",
                    subscriber = new
                    {
                        country = "UG",
                        currency = "UGX",
                        msisdn = SanitiseUgMobileNumber(BookingUser.Telephone)
                    },
                    transaction = new
                    {
                        amount = Amount.ToString("0.##"),
                        country = "UG",
                        currency = "UGX",
                        id = Title
                    }
                };
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
                var response = client.Execute<dynamic>(request);

                if (AirtelLog)
                    Entities.Log.Debug(
                        "Transaction.RequestAirtelPayment",
                        client.BaseUrl +
                        Environment.NewLine +
                        Environment.NewLine +
                        JsonConvert.SerializeObject(response) +
                        Environment.NewLine +
                        Environment.NewLine);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var response_code = response.Data["status"]["code"];
                    var response_message = response.Data["status"]["message"].ToString();

                    if (response_code == "DP00800001001")
                    {
                        StatusId = TransactionStatus.Completed;
                        Save();
                        PaymentComplete();
                        return true;
                    }
                    else if (response_code == "DP00800001006")
                    {
                        StatusId = TransactionStatus.PendingSubscriberValidation;
                        Save();
                        return true;
                    }
                    else
                    {
                        Error = response_message;
                        StatusId = TransactionStatus.Error;
                        Save();
                        return false;
                    }                    
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("Transaction.RequestAirtelPayment", ex);
            }

            Error = "An unknown error occurred";
            StatusId = TransactionStatus.Error;
            Save();
            return false;
        }

        public bool RequestAirtelRefund()
        {
            var originalId = Id;
            Id = 0;

            if (!LivePayments)
            {
                RefundComplete(originalId);
                return true;
            }

            try
            {
                var client = new RestClient($"{AirtelUrl}/standard/v1/payments/refund");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("X-Country", "UG");
                request.AddHeader("X-Currency", "UGX");
                request.AddHeader("Authorization", $"Bearer {AirtelAccessToken}");
                var body = new
                {
                    transaction = new
                    {
                        airtel_money_id = EcoCashReference
                    }
                };
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
                var response = client.Execute<dynamic>(request);

                if (AirtelLog)
                    Entities.Log.Debug(
                        "Transaction.RequestAirtelRefund",
                        client.BaseUrl +
                        Environment.NewLine +
                        Environment.NewLine +
                        JsonConvert.SerializeObject(response) +
                        Environment.NewLine +
                        Environment.NewLine);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var status = response.Data["data"]["transaction"]["status"];
                    var airtel_money_id = response.Data["data"]["transaction"]["airtel_money_id"];
                    var response_message = response.Data["status"]["message"].ToString();

                    if (status == "SUCCESS")
                    {
                        StatusId = TransactionStatus.Completed;
                        RefundComplete(originalId);
                        return true;
                    }
                    else
                    {
                        Error = response_message;
                        StatusId = TransactionStatus.Error;
                        Save();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("Transaction.RequestAirtelRefund", ex);
            }

            Error = "An unknown error occurred";
            StatusId = TransactionStatus.Error;
            Save();
            return false;
        }

        public void RequestAirtelStatus()
        {
            var previousStatusId = StatusId;

            if (!LivePayments)
            {
                StatusId = TransactionStatus.Completed;
                EcoCashReference = "DEM-" + Guid.NewGuid().ToString();
            }
            else
            {
                try
                {
                    var client = new RestClient($"{AirtelUrl}/standard/v1/payments/{Title}");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Cache-Control", "no-cache");
                    request.AddHeader("X-Country", "UG");
                    request.AddHeader("X-Currency", "UGX");
                    request.AddHeader("Authorization", $"Bearer {AirtelAccessToken}");

                    var response = client.Execute<dynamic>(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var response_code = response.Data["status"]["code"];
                        var response_message = response.Data["status"]["message"];
                        var airtel_money_id = response.Data["transaction"]["airtel_money_id"];

                        if (response_code == "DP00800001001")
                        {
                            StatusId = TransactionStatus.Completed;
                            EcoCashReference = airtel_money_id;
                        }
                        else if (response_code == "DP00800001006")
                        {
                            StatusId = TransactionStatus.PendingSubscriberValidation;
                        }
                        else
                        {
                            Error = response_message;
                            StatusId = TransactionStatus.Error;
                        }
                    }
                    else
                    {
                        Error = "An unknown error occurred";
                        StatusId = TransactionStatus.Error;
                    }
                }
                catch (Exception ex)
                {
                    Entities.Log.Error("Transaction.RequestAirtelStatus", ex);
                }
            }

            Save();

            if (previousStatusId != TransactionStatus.Completed && StatusId == TransactionStatus.Completed)
                PaymentComplete();
        }

        #endregion

        #region EcoCash

        private static string SanitiseZwMobileNumber(string MobileNumber)
        {
            if (MobileNumber.StartsWith("0"))
                return Regex.Replace(Regex.Replace(MobileNumber, "^0", "263"), " ", "");

            if (!MobileNumber.StartsWith("263"))
                return "263" + Regex.Replace(MobileNumber, " ", "");

            return MobileNumber;
        }

        private static string EcoCashUrl => Config.Find(Key: "EcoCash URL").Value;
        private static string EcoCashMerchantCode => Config.Find(Key: "EcoCash Merchant Code").Value;
        private static string EcoCashMerchantPin => Config.Find(Key: "EcoCash Merchant Pin").Value;
        private static string EcoCashMerchantNumber => Config.Find(Key: "EcoCash Merchant Number").Value;
        private static string EcoCashUsername => Config.Find(Key: "EcoCash Username").Value;
        private static string EcoCashPassword => Config.Find(Key: "EcoCash Password").Value;
        private static bool EcoCashLog => Config.Find(Key: "EcoCash Log").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        public bool RequestEcoCashPayment()
        {
            Save();

            if (!LivePayments)
            {
                ServerReference = "DEMO-" + Guid.NewGuid().ToString();
                StatusId = TransactionStatus.PendingSubscriberValidation; // Updated when polled
                Save();

                return true;
            }

            var resourceUri = $"{EcoCashUrl}transactions/amount";

            var json = JsonConvert.SerializeObject(new
            {
                clientCorrelator = ClientCorrelator,
                notifyUrl = $"http://197.211.236.157:9800/transactions/ecocash/notify",
                referenceCode = Title,
                tranType = "MER",
                endUserId = SanitiseZwMobileNumber(BookingUser.Telephone),
                remarks = Config.ApplicationName,
                transactionOperationStatus = "Charged",
                paymentAmount = new
                {
                    charginginformation = new
                    {
                        amount = Amount.ToString("0.##"),
                        currency = "RTGS",
                        description = Booking?.Listing?.Title ?? "AgriShare Booking"
                    },
                    chargeMetaData = new
                    {
                        channel = "WEB",
                        purchaseCategoryCode = "Booking",
                        onBeHalfOf = Config.ApplicationName
                    }
                },
                merchantCode = EcoCashMerchantCode,
                merchantPin = EcoCashMerchantPin,
                merchantNumber = EcoCashMerchantNumber,
                currencyCode = "RTGS",
                countryCode = "ZW",
                terminalID = "001",
                location = "api.agrishare.app",
                superMerchantName = "WHH",
                merchantName = Config.ApplicationName
            });

            if (EcoCashLog)
                Log += resourceUri + Environment.NewLine + Environment.NewLine + json;

            var client = new RestClient(resourceUri)
            {
                Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            var response = client.Execute<dynamic>(request);

            if (EcoCashLog)
                Log += Environment.NewLine + Environment.NewLine + JsonConvert.SerializeObject(response);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    Error = response.Data["text"];
                }
                catch { }

                try
                {
                    ServerReference = response.Data["serverReferenceCode"];
                    StatusId = TransactionStatus.PendingSubscriberValidation;
                    Save();
                    return true;
                }
                catch
                {
                    StatusId = TransactionStatus.Error;
                    Save();
                    return false;
                }
            }
            else
            {
                StatusId = TransactionStatus.Error;
                Save();
                return false;
            }
        }

        public void RequestEcoCashStatus()
        {
            var previousStatusId = StatusId;

            if (LivePayments)
            {
                var resourceUri = $"{EcoCashUrl}{SanitiseZwMobileNumber(BookingUser.Telephone)}/transactions/amount/{ClientCorrelator}";

                var client = new RestClient(resourceUri)
                {
                    Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
                };

                var request = new RestRequest(Method.GET);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                var response = client.Execute<dynamic>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        Error = response.Data["text"];
                    }
                    catch { }

                    try
                    {
                        var status = response.Data["transactionOperationStatus"];
                        if (status == "COMPLETED")
                            StatusId = TransactionStatus.Completed;
                        else if (status == "PENDING SUBSCRIBER VALIDATION")
                            StatusId = TransactionStatus.PendingSubscriberValidation;
                        else if (status == "TRANSACTION TIMEDOUT")
                            StatusId = TransactionStatus.TransactionTimedout;
                        else
                            StatusId = TransactionStatus.Failed;

                        if (StatusId == TransactionStatus.Completed)
                        {
                            if (EcoCashLog)
                                Log += Environment.NewLine + Environment.NewLine +
                                    resourceUri + Environment.NewLine + Environment.NewLine +
                                    JsonConvert.SerializeObject(response);

                            ServerReference = response.Data["serverReferenceCode"];
                            EcoCashReference = response.Data["ecocashReference"];
                        }

                    }
                    catch
                    {
                        StatusId = TransactionStatus.Error;
                        Save();
                        return;
                    }
                }
                else
                    StatusId = TransactionStatus.Error;

            }
            else
            {
                StatusId = TransactionStatus.Completed;
                EcoCashReference = "ECO-" + Guid.NewGuid().ToString();
            }

            Save();

            if (previousStatusId != TransactionStatus.Completed && StatusId == TransactionStatus.Completed)
                PaymentComplete();
        }

        public bool RequestEcoCashRefund()
        {
            var originalId = Id;

            Id = 0;
            ClientCorrelator = Guid.NewGuid().ToString();
            Log = string.Empty;

            if (!LivePayments)
            {
                RefundComplete(originalId);
                return true;
            }

            var resourceUri = $"{EcoCashUrl}transactions/refund";

            var json = JsonConvert.SerializeObject(new
            {
                clientCorrelator = ClientCorrelator,
                notifyUrl = $"{Config.APIURL}/transactions/ecocash/notify",
                referenceCode = Title,
                tranType = "REF",
                endUserId = SanitiseZwMobileNumber(BookingUser.Telephone),
                remarks = Config.ApplicationName,
                transactionOperationStatus = "Refunded",
                originalEcocashReference = EcoCashReference,
                paymentAmount = new
                {
                    charginginformation = new
                    {
                        amount = Amount.ToString("0.##"),
                        currency = "RTGS",
                        description = Booking?.Listing?.Title ?? "AgriShare Booking"
                    },
                    chargeMetaData = new
                    {
                        channel = "WEB",
                        purchaseCategoryCode = "Booking",
                        onBeHalfOf = Config.ApplicationName
                    }
                },
                merchantCode = EcoCashMerchantCode,
                merchantPin = EcoCashMerchantPin,
                merchantNumber = EcoCashMerchantNumber,
                currencyCode = "RTGS",
                countryCode = "ZW",
                terminalID = "001",
                location = "api.agrishare.app",
                superMerchantName = "WHH",
                merchantName = Config.ApplicationName
            });

            if (EcoCashLog)
                Log += resourceUri + Environment.NewLine + json;

            var client = new RestClient(resourceUri)
            {
                Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            var response = client.Execute<dynamic>(request);

            if (EcoCashLog)
                Log += Environment.NewLine + Environment.NewLine + JsonConvert.SerializeObject(response);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    Error = response.Data["text"];
                }
                catch { }

                try
                {
                    var status = response.Data["transactionOperationStatus"];

                    if (status == "COMPLETED")
                        StatusId = TransactionStatus.Completed;
                    else if (status == "PENDING SUBSCRIBER VALIDATION")
                        StatusId = TransactionStatus.PendingSubscriberValidation;
                    else if (status == "TRANSACTION TIMEDOUT")
                        StatusId = TransactionStatus.TransactionTimedout;
                    else
                        StatusId = TransactionStatus.Failed;

                    if (StatusId == TransactionStatus.Completed)
                        RefundComplete(originalId);

                    Save();

                    return true;
                }
                catch
                {
                    StatusId = TransactionStatus.Error;
                    Save();
                    return false;
                }
            }
            else
            {
                StatusId = TransactionStatus.Error;
                Save();
                return false;
            }
        }

        #endregion

        #region Random String

        public static string GenerateRandomCode(int length)
        {
            //Initiate objects & vars    
            String randomString = "";
            int randNumber;

            //Loop ‘length’ times to generate a random number or character
            for (int i = 0; i < length; i++)
            {
                int _next = random.Next(1, 3);
                if (_next == 1)
                    randNumber = random.Next(97, 123); //char {a-z}
                else if (_next == 2)
                    randNumber = random.Next(65, 91); //char {A-Z}
                else
                    randNumber = random.Next(48, 58); //int {0-9}

                //append random char or digit to random string
                randomString = randomString + (char)randNumber;
            }
            //return the random string
            return randomString;
        }
        private static Random random = new Random();

        #endregion
    }

}