using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Collections.Generic;
using Agrishare.Core.Entities;
using Agrishare.Core.Utils;

namespace Agrishare.API
{
    public class Authorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Authentication required");
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var allowedRoles = Roles.Split(',').Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true));

            var authorizationHeader = actionContext.Request.Headers.Where(e => e.Key == "Authorization").FirstOrDefault();
            var encryptedToken = authorizationHeader.Value.FirstOrDefault() ?? string.Empty;
            var token = Encryption.DecryptWithRC4(encryptedToken, Config.EncryptionSalt);

            var user = User.Find(AuthToken: token);
            var allowed = user.Roles.Intersect(allowedRoles).Count() > 0;

            return allowed;
        }
    }
}