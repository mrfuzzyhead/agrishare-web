using Agrishare.CMS.Code;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.CMS
{
    public partial class Default : BasePage
    {
        public DateTime Today = DateTime.Today;

        protected override void OnPreRender(EventArgs e)
        {
            var version = Config.Find(Key: "Resource Version")?.Value ?? "1";

            Page.Header.Controls.Add(new Literal { Text = $@"<link rel=""stylesheet"" href=""/styles-{version}.css"" />" });
            Page.Header.Controls.Add(new Literal { Text = $@"<script type=""text/javascript"" src=""/script-{version}.js""></script>" });

            Body.Attributes.Add("ag-api-url", Config.APIURL);
            Body.Attributes.Add("ng-init", $"app.user={CurrentUser.CmsJsonString()};app.region={CurrentRegion.CmsJsonString()};app.regions={Region.CmsListJsonString()};app.cookieDomain='{Config.DomainName}';");

            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var userRoles = CurrentUser.Roles ?? new List<Role>();

            if (Request.QueryString["logout"] == "true")
            {
                var user = Core.Entities.User.Find(Id: CurrentUser.Id);
                user.AuthToken = string.Empty;
                user.Save();

                CurrentUser = null;

                var cookie = HttpContext.Current.Request.Cookies[Core.Entities.User.AuthCookieName];
                cookie.Value = string.Empty;
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Domain = Config.DomainName;
                HttpContext.Current.Response.Cookies.Add(cookie);

                Response.Redirect("/login.aspx");
            }
            else if (!userRoles.Contains(Role.Administrator) && !userRoles.Contains(Role.Dashboard))
                Response.Redirect("/login.aspx");
        }

    }
}