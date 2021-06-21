using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Irrigation : Page
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
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            var url = $"/account/seeking/results?cid={Core.Entities.Category.IrrigationId}&" +
                $"sid={Core.Entities.Category.IrrigationId}&" +
                $"lat={Location.Latitude}&" +
                $"lng={Location.Longitude}&" +
                $"std={StartDate.Text}&" +
                $"mob=1&" +
                $"qty={DayCount.Text}&" +
                $"dis={WaterSourceDistance.Text}&" +
                $"dep={WaterSourceDepth.Text}&" +
                $"for={For.SelectedValue}";

            Response.Redirect(url);
        }
    }
}