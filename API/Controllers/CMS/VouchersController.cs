using Agrishare.Core;
using Agrishare.Core.Entities;
using System.Linq;
using System.Web.Http;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class VouchersController : BaseApiController
    {
        [Route("vouchers/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Keywords = "")
        {
            var recordCount = Voucher.Count(Keywords: Keywords);
            var list = Voucher.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Keywords);

            var data = new
            {
                Count = recordCount,
                Sort = Voucher.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Vouchers"
            };

            return Success(data);
        }

        [Route("vouchers/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var voucher = Voucher.Find(Id: Id);
            if (voucher.Id > 0 && voucher.TypeId == VoucherType.Percentage)
                voucher.Amount *= 100;

            var data = new
            {
                Entity = voucher.Json(),                
                Types = EnumInfo.ToList<VoucherType>().Where(g => g.Title != "None").ToList()
            };

            return Success(data);
        }

        [Route("vouchers/save")]
        [AcceptVerbs("POST")]
        public object Save(Voucher Voucher)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Voucher.TypeId == VoucherType.Percentage)
                Voucher.Amount /= 100;

            if (!Voucher.UniqueCode())
                return Error("The selected voucher code has already been used");

            if (Voucher.Save())
                return Success(new
                {
                    Entity = Voucher.Json()
                });

            return Error();
        }

        [Route("vouchers/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var voucher = Voucher.Find(Id: Id);

            if (voucher.Delete())
                return Success(new
                {
                    Entity = voucher.Json()
                });

            return Error();
        }
    }
}
