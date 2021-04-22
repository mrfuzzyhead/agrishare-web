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
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SendMessage(object s, EventArgs e)
        {
            new Core.Entities.Message
            {
                Content = Message.Text,
                EmailAddress = EmailAddress.Text,
                Name = Name.Text,
                Telephone = Telephone.Text,
                Title = Subject.Text
            }.Save();

            Master.Feedback = "Your message has been sent";
            Response.Redirect("/about/contact");
        }
    }
}