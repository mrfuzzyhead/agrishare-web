using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class AdvertsController : BaseApiController
    {
        [Route("ad/impression")]
        [AcceptVerbs("GET")]
        public object AddImpression(int Id)
        {
            Entities.Advert.AddImpression(Id);
            return Success();
        }

        [Route("ad/click")]
        [AcceptVerbs("GET")]
        public object AddClick(int Id)
        {
            Entities.Advert.AddClick(Id);
            return Success();
        }
    }
}
