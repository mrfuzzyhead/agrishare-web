using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.About
{
    public partial class Contact : System.Web.UI.Page
    {
        // Load
        protected void Page_Load(object sender, EventArgs e)
        {
            var script = new Literal { Text = $"<script src=\"https://www.google.com/recaptcha/api.js\" async defer></script>" };
            Master.Head.Controls.Add(script);

            if (!Page.IsPostBack)
            {
                Region.DataSource = Core.Entities.Region.List();
                Region.DataTextField = "Title";
                Region.DataValueField = "Id";
                Region.DataBind();
                Region.Items.Insert(0, new ListItem("Select", ""));
            }
        }

        public void SendMessage(object s, EventArgs e)
        {
            if (Page.IsValid && ValidCaptcha())
            {
                new Core.Entities.Message
                {
                    Content = Message.Text,
                    EmailAddress = EmailAddress.Text,
                    Name = Name.Text,
                    Telephone = Telephone.Text,
                    Title = Subject.Text,
                    RegionId = Convert.ToInt32(Region.SelectedValue)
                }.Save();

                Master.Feedback = "Your message has been sent";
                Response.Redirect("/about/contact");
            }
            else
            {
                Master.Feedback = "Could not send message, please try again";
                Response.Redirect("/about/contact");
            }
        }


        public string RecaptchaSiteKey = "6LfZP7gaAAAAAJY78B59pYa11a-0nPk9R-bSj8IW";
        private string RecaptchaSecretKey = "6LfZP7gaAAAAAGmRIXxywUN0NgylcZdYFtoUylJO";

        private bool ValidCaptcha()
        {
            var token = Request.Form["g-recaptcha-response"];

            var request = new RestRequest(Method.POST);
            request.AddParameter("secret", RecaptchaSecretKey);
            request.AddParameter("response", token);
            var client = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            var response = client.Execute<RecpatchaResponse>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data.success)
                return true;

            return false;
        }

        class RecpatchaResponse
        {
            public bool success { get; set; }
        }
    }
}