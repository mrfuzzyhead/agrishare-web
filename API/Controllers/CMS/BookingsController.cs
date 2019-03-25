using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsBookingsController : BaseApiController
    {
        [Route("bookings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "", int UserId = 0)
        {
            var recordCount = Entities.Booking.Count(UserId: UserId);
            var list = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: UserId);
            var title = "Bookings";

            if (UserId > 0)
            {
                var user = Entities.User.Find(Id: UserId);
                if (user != null)
                    title = $"{user.FirstName} {user.LastName}";
            }

            int bookingsMadeMe = 0, bookingsMadeFriend = 0, bookingsMadeGroup = 0;
            int confirmedBookingsMe = 0, confirmedBookingsFriend = 0, confirmedBookingsGroup = 0;
            int paidBookingsMe = 0, paidBookingsFriend = 0, paidBookingsGroup = 0;
            int completedBookingsMe = 0, completedBookingsFriend = 0, completedBookingsGroup = 0;
            int declinedBookings = 0, cancelledBookings = 0, incompleteBookings = 0;
            if (PageIndex == 0)
            {
                bookingsMadeMe = Entities.Counter.Count(Event: Entities.Counters.Book, For: Entities.BookingFor.Me);
                confirmedBookingsMe = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, For: Entities.BookingFor.Me);
                paidBookingsMe = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, For: Entities.BookingFor.Me);
                completedBookingsMe = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, For: Entities.BookingFor.Me);

                bookingsMadeFriend = Entities.Counter.Count(Event: Entities.Counters.Book, For: Entities.BookingFor.Friend);
                confirmedBookingsFriend = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, For: Entities.BookingFor.Friend);
                paidBookingsFriend = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, For: Entities.BookingFor.Friend);
                completedBookingsFriend = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, For: Entities.BookingFor.Friend);

                bookingsMadeGroup = Entities.Counter.Count(Event: Entities.Counters.Book, For: Entities.BookingFor.Group);
                confirmedBookingsGroup = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, For: Entities.BookingFor.Group);
                paidBookingsGroup = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, For: Entities.BookingFor.Group);
                completedBookingsGroup = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, For: Entities.BookingFor.Group);

                declinedBookings = Entities.Counter.Count(Event: Entities.Counters.DeclineBooking);
                cancelledBookings = Entities.Counter.Count(Event: Entities.Counters.CancelBooking);
                incompleteBookings = Entities.Counter.Count(Event: Entities.Counters.IncompleteBooking);
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title,
                Summary = new
                {
                    BookingsMadeMe = bookingsMadeMe,
                    ConfirmedBookingsMe = confirmedBookingsMe,
                    PaidBookingsMe = paidBookingsMe,
                    CompletedBookingsMe = completedBookingsMe,

                    BookingsMadeFriend = bookingsMadeFriend,
                    ConfirmedBookingsFriend = confirmedBookingsFriend,
                    PaidBookingsFriend = paidBookingsFriend,
                    CompletedBookingsFriend = completedBookingsFriend,

                    BookingsMadeGroup = bookingsMadeGroup,
                    ConfirmedBookingsGroup = confirmedBookingsGroup,
                    PaidBookingsGroup = paidBookingsGroup,
                    CompletedBookingsGroup = completedBookingsGroup,

                    DeclinedBookings = declinedBookings,
                    CancelledBookings = cancelledBookings,
                    IncompleteBookings = incompleteBookings
                }
            };

            return Success(data);
        }

        [Route("bookings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Booking.Find(Id: Id).Json(),
                Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
            };

            return Success(data);
        }

        [Route("bookings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Booking Booking)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Booking.Save())
                return Success(new
                {
                    Entity = Booking.Json()
                });

            return Error();
        }

        [Route("bookings/transactions/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);
            foreach (var transaction in transactions)
                transaction.RequestEcoCashStatus();

            var data = new
            {
                Entity = booking.Json(),
                Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                Feedback = $"Finished updating transaction statuses"
            };

            return Success(data);
        }

        [Route("bookings/cancel")]
        [AcceptVerbs("GET")]
        public object CancelBooking(int Id = 0)
        {
            var booking = Entities.Booking.Find(Id: Id);
            if (booking == null || booking.Id == 0)
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("You can not cancel a completed booking");

            if (booking.StatusId == Entities.BookingStatus.InProgress)
                return Error("You can not cancel a paid booking that is not complete");

            booking.StatusId = Entities.BookingStatus.Cancelled;
            if (booking.Save())
            {
                var transactions = Entities.Transaction.List(BookingId: booking.Id);
                foreach (var transaction in transactions)
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

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: false);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.CancelBooking, booking.Service.CategoryId, booking.Id);

                var data = new
                {
                    Entity = booking.Json(),
                    Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                    Feedback = $"Booking was cancelled"
                };

                return Success(data);
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/transactions/refund")]
        [AcceptVerbs("GET")]
        public object RefundTransaction(int Id = 0)
        {
            var transaction = Entities.Transaction.Find(Id: Id);

            if (transaction.RequestEcoCashRefund())
            {
                var data = new
                {
                    Entity = Entities.Booking.Find(Id: Id).Json(),
                    Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
                };

                return Success(data);
            }
            else
                return Error("Unable to process refund");
        }
    }
}
