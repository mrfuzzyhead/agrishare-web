/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */
 
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Collections.Generic;
using Agrishare.Core.Entities;
using Agrishare.Core.Utils;
using Agrishare.Core;

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
            User currentUser = null;
            Region currentRegion = null;

            #region Find current user

            var token = string.Empty;
            var userCookies = actionContext.Request.Headers.GetCookies(User.AuthCookieName).FirstOrDefault();
            var encryptedToken = userCookies?.Cookies.FirstOrDefault(e => e.Name == User.AuthCookieName)?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(encryptedToken))
                token = Encryption.DecryptWithRC4(encryptedToken, Config.EncryptionSalt);
            else if (actionContext.Request.Headers.TryGetValues("Authorization", out var authorizationValues))
                token = authorizationValues.First();
            else
            {
                var queryString = actionContext.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (queryString != null && queryString.Keys.Contains("Authorization"))
                    token = queryString["Authorization"];
            }

            currentUser = User.Find(AuthToken: token);

            #endregion

            #region Find current region

            var regionCookies = actionContext.Request.Headers.GetCookies("region").FirstOrDefault();
            var regionId = regionCookies?.Cookies.FirstOrDefault(e => e.Name == "region")?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(regionId))
                currentRegion = Region.Find(Convert.ToInt32(regionId));
            if (currentRegion == null || currentRegion.Id == 0)
                currentRegion = currentUser.Region;

            #endregion

            actionContext.Request.Properties["CurrentUser"] = currentUser;
            actionContext.Request.Properties["CurrentRegion"] = currentRegion;

            var allowedRoles = Roles.Split(',').Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true));
            if (allowedRoles.Count() == 0)
                return true;

            return currentUser?.Roles.Intersect(allowedRoles).Count() > 0;            
        }
    }
}