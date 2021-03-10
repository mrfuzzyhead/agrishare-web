using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Labour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";
            Master.SelectedUrl = "/account/seeking";

            if (!Page.IsPostBack)
            {
                For.Items.Add(new ListItem { Text = "Me", Value = ((int)BookingFor.Me).ToString() });
                For.Items.Add(new ListItem { Text = "A friend", Value = ((int)BookingFor.Friend).ToString() });
                For.Items.Add(new ListItem { Text = "A group", Value = ((int)BookingFor.Group).ToString() });

                Services.Items.Add(new ListItem("Harvesting", $"{(int)LabourService.Harvesting}"));
                Services.Items.Add(new ListItem("Land Clearing", $"{(int)LabourService.LandClearing}"));
                Services.Items.Add(new ListItem("Loading", $"{(int)LabourService.Loading}"));
                Services.Items.Add(new ListItem("Planting", $"{(int)LabourService.Planting}"));
                Services.Items.Add(new ListItem("Weeding", $"{(int)LabourService.Weeding}"));
                Services.Items.Add(new ListItem("Other", $"{(int)LabourService.Other}"));
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            int services = 0;
            foreach (ListItem item in Services.Items)
                if (item.Selected)
                    services += Convert.ToInt32(item.Value);

            var url = $"/account/seeking/results?cid={Category.LabourId}&" +
                $"sid={Category.LabourId}&" +
                $"lat={Location.Latitude}&" +
                $"lng={Location.Longitude}&" +
                $"std={StartDate.Text}&" +
                $"qty={DayCount.Text}&" +
                $"mob=1&" +
                $"lsr={services}&" +
                $"for={For.SelectedValue}";

            Response.Redirect(url);
        }
    }
}