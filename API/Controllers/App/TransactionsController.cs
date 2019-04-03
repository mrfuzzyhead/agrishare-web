﻿using Agrishare.API.Models;
using Agrishare.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class TransactionsController : BaseApiController
    {
        [Route("transactions/ecocash/notify")]
        [AcceptVerbs("POST")]
        public object EcoCashNotification(EcoCashModel Model)
        {
            if (Model.TransactionOperationStatus == "COMPLETED")
                Entities.Transaction.Find(ClientCorrelator: Model.ClientCorrelator).RequestEcoCashStatus();

            return Success(Model);
        }

        [@Authorize(Roles = "User")]
        [Route("transactions/ecocash/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id).Where(e => e.StatusId != Entities.TransactionStatus.Error);
            if (transactions.Count() == 0)
                return Error("There are no transactions for this booking");

            foreach (var transaction in transactions)
                if (transaction.StatusId == Entities.TransactionStatus.PendingSubscriberValidation)
                    transaction.RequestEcoCashStatus();

            var bookingUsers = Entities.BookingUser.List(BookingId: BookingId);
            if (bookingUsers.Count(e => e.StatusId != Entities.BookingUserStatus.Paid) == 0)
                return Success("Payment complete");

            // NB assumes there is only one transaction required per booking (no more group users)
            var lastTransaction = transactions.OrderByDescending(e => e.DateCreated).First();
            if (lastTransaction.StatusId == Entities.TransactionStatus.PendingSubscriberValidation)
                return Success(new
                {
                    Transactions = new List<object>()
                });

            return Error(lastTransaction.Status);
        }

        [@Authorize(Roles = "User")]
        [Route("transactions/create")]
        [AcceptVerbs("POST")]
        public object CreateTransaction(TransactionModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var booking = Entities.Booking.Find(Id: Model.BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            booking.BookingUsers = Entities.BookingUser.List(BookingId: booking.Id);

            var bookingUsers = new List<Core.Entities.BookingUser>();
            foreach (var user in Model.Users)
            {
                if (user == null)
                    continue;

                if (user.Quantity == 0)
                    user.Quantity = 1;
                if (booking.Quantity == 0)
                    booking.Quantity = 1;

                var bookingUser = booking.BookingUsers.FirstOrDefault(o => o.Telephone == user.Telephone);
                if (bookingUser == null)
                {
                    bookingUser = new Entities.BookingUser
                    {
                        Booking = booking,
                        Name = user.Name,
                        Ratio = user.Quantity / booking.Quantity,
                        StatusId = Entities.BookingUserStatus.Pending,
                        Telephone = user.Telephone
                    };

                    var registeredUser = Entities.User.Find(Telephone: user.Telephone);
                    if (registeredUser.Id > 0)
                        bookingUser.User = registeredUser;

                    bookingUser.Save();
                    booking.BookingUsers.Add(bookingUser);
                }
                else
                {
                    bookingUser.Ratio = user.Quantity / booking.Quantity;

                    var registeredUser = Entities.User.Find(Telephone: user.Telephone);
                    if (registeredUser.Id > 0)
                        bookingUser.User = registeredUser;

                    bookingUser.Save();
                }

                bookingUsers.Add(bookingUser);
            }

            foreach (var bookingUser in booking.BookingUsers)
                if (bookingUsers.Count(e => e.Id == bookingUser.Id) == 0)
                    bookingUser.Delete();

            var success = true;
            var errorMessage = string.Empty;
            var transactions = Entities.Transaction.List(BookingId: booking.Id);
            foreach(var transaction in transactions.Where(e => e.StatusId == Entities.TransactionStatus.Failed))
            {
                transaction.StatusId = Entities.TransactionStatus.Error;
                transaction.Save();
            }
            
            foreach (var bookingUser in bookingUsers)
            {
                var transaction = transactions.FirstOrDefault(o =>
                    o.BookingUserId == bookingUser.Id &&
                    o.StatusId != Entities.TransactionStatus.Error &&
                    o.StatusId != Entities.TransactionStatus.Failed);

                if (transaction == null)
                {
                    transaction = new Entities.Transaction
                    {
                        Amount = booking.Price * bookingUser.Ratio,
                        Booking = booking,
                        BookingUser = bookingUser,
                        StatusId = Entities.TransactionStatus.Pending
                    };

                    transaction.Save();
                    transaction.RequestEcoCashPayment();
                    transactions.Add(transaction);

                    Entities.Counter.Hit(bookingUser.UserId ?? 0, Entities.Counters.InitiatePayment, booking.Service.CategoryId, booking.Id);

                    if (transaction.StatusId != Entities.TransactionStatus.PendingSubscriberValidation)
                    {
                        success = false;
                        errorMessage = $"{bookingUser.Telephone}: {transaction.Error}";
                    }
                }
            }

            if (success && transactions.Count > 0)
                return Success(new
                {
                    Transactions = transactions.Select(e => e.Json())
                });
            else
                return Error(errorMessage.Coalesce("No transactions were created"));
        }

        [@Authorize(Roles = "User")]
        [Route("transactions")]
        [AcceptVerbs("GET")]
        public object TransactionList(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);

            return Success(new
            {
                Transactions = transactions.Select(e => e.Json())
            });
        }
    }
}
