using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.About
{
    public partial class Thread : System.Web.UI.Page
    {
        Core.Entities.Message SelectedMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            SelectedMessage = Core.Entities.Message.Find(Request.QueryString["guid"]);
            if (SelectedMessage == null)
            {
                Master.Feedback = "Message not found";
                Response.Redirect("/about/contact");
            }

            MessageTitle.Text = HttpUtility.HtmlEncode(SelectedMessage.Title);
            MessageContent.Text = HttpUtility.HtmlEncode(SelectedMessage.Content);

            ThreadList.DataSource = Core.Entities.Message.List(ParentId: SelectedMessage.Id, Sort: "Id");
            ThreadList.DataBind();
        }

        public void BindMessage(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var message = (Core.Entities.Message)e.Item.DataItem;
                ((Literal)e.Item.FindControl("Name")).Text = HttpUtility.HtmlEncode(message.User?.Title ?? message.Name);
                ((Literal)e.Item.FindControl("Date")).Text = message.Date.ToString("d MMMM yyyy h:mmtt");
                ((Literal)e.Item.FindControl("Content")).Text = HttpUtility.HtmlEncode(message.Content);
            }
        }

        public void SendReply(object s, EventArgs e)
        {
            new Core.Entities.Message
            {
                ParentId = SelectedMessage.Id,
                Content = Message.Text,
                Name = SelectedMessage.Name,
                EmailAddress = SelectedMessage.EmailAddress,
                Telephone = SelectedMessage.Telephone,
                Title = $"REPLY: {SelectedMessage.Title}"
            }.Save();

            SelectedMessage.StatusId = Core.Entities.MessageStatus.Unread;
            SelectedMessage.Date = DateTime.UtcNow;
            SelectedMessage.Save();

            Master.Feedback = "Your reply has been sent";
            Response.Redirect($"/about/thread?guid={SelectedMessage.GUID}");
        }
    }
}