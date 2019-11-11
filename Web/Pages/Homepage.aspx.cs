﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages
{
    public partial class Homepage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Body.Attributes["class"] += " home ";

            ListingCount.Text = Core.Entities.Listing.Count(Status: Core.Entities.ListingStatus.Live).ToString();
            UserCount.Text = Core.Entities.User.Count(Status: Core.Entities.UserStatus.Verified).ToString();
        }
    }
}