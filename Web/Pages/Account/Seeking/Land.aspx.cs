using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Land : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";
            Master.SelectedUrl = "/account/seeking";

            if (!Page.IsPostBack)
            {
                For.Items.Add(new ListItem { Text = "Me", Value = ((int)Core.Entities.BookingFor.Me).ToString() });
                For.Items.Add(new ListItem { Text = "A friend", Value = ((int)Core.Entities.BookingFor.Friend).ToString() });
                For.Items.Add(new ListItem { Text = "A group", Value = ((int)Core.Entities.BookingFor.Group).ToString() });

                LandRegion.Items.Add(new ListItem { Text = "Central", Value = ((int)Core.Entities.ServiceLandRegion.Central).ToString() });
                LandRegion.Items.Add(new ListItem { Text = "Eastern", Value = ((int)Core.Entities.ServiceLandRegion.Eastern).ToString() });
                LandRegion.Items.Add(new ListItem { Text = "Northern", Value = ((int)Core.Entities.ServiceLandRegion.Northern).ToString() });
                LandRegion.Items.Add(new ListItem { Text = "Western", Value = ((int)Core.Entities.ServiceLandRegion.Western).ToString() });
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            var url = $"/account/seeking/results?cid={Core.Entities.Category.LandId}&" +
                $"sid={Core.Entities.Category.LandId}&" +
                $"std={StartDate.Text.ToString()}&" +
                $"reg={LandRegion.SelectedValue}&" +
                $"vol={Acres.Text}&" +
                $"qty={Years.Text}&" +
                $"for={For.SelectedValue}";

            Response.Redirect(url);
        }
    }
}