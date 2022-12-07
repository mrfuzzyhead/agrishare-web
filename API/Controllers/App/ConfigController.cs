using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class ConfigController : BaseApiController
    {
        [Route("config/currentversion")]
        [AcceptVerbs("GET")]
        public object CurrentVersion()
        {
            return Success(new
            {
                Version = Entities.Config.Find(Key: "App Version")?.Value ?? "1.0.0"
            });
        }

        [Route("config/contactdetails")]
        [AcceptVerbs("GET")]
        public object ContactDetails()
        {
            return Success(new
            {
                Text = Entities.Config.Find(Key: "Contact Details")?.Value
            });
        }
    }
}
