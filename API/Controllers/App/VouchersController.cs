using Agrishare.API.Models;
using Agrishare.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles = "User")]
    public class VouchersController : BaseApiController
    {
        [Route("voucher/validate")]
        [AcceptVerbs("GET")]
        public object ApplyVoucher(string Code, int BookingId)
        {
            if (CurrentUser.FailedVoucherAttempts > Entities.User.MaxFailedVoucherAttempts)
                return Error("You have too many failed voucher attempts - please contact AgriShare to redeem vouchers in future");

            var voucher = Entities.Voucher.Find(Code: Code);
            if (voucher == null)
            {
                CurrentUser.FailedVoucherAttempts += 1;
                CurrentUser.Save();

                return Error("Voucher not found");
            }

            if (voucher.RedeemCount < voucher.MaxRedeemCount)
            {
                var booking = Entities.Booking.Find(BookingId);
                if (booking.UserId != CurrentUser.Id)
                    return Error("Booking not found");

                booking.VoucherId = voucher.Id;

                var voucherableAmount = booking.HireCost + booking.TransportCost;
                booking.VoucherTotal = voucher.TypeId == Entities.VoucherType.Value ? voucher.Amount : voucherableAmount * voucher.Amount;
                booking.Price = booking.HireCost + booking.FuelCost + booking.TransportCost - booking.VoucherTotal;

                if (booking.Save())
                {
                    voucher.RedeemCount += 1;
                    voucher.Save();

                    return Success(new
                    {
                        voucher.Id,
                        voucher.Amount,
                        voucher.TypeId
                    });
                }
            }

            return Error("Voucher has already been redeemed");
        }
    }
}
