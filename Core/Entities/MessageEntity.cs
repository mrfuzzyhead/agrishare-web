using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Message : IEntity
    {
        public static string DefaultSort = "Date DESC";
        public string Status => $"{StatusId}".ExplodeCamelCase();

        public static Message Find(int Id)
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Messages
                    .Include(e => e.User)
                    .Where(o => !o.Deleted)
                    .Where(e => e.Id == Id)
                    .FirstOrDefault();
        }

        public static Message Find(string GUID)
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Messages
                    .Include(e => e.User)
                    .Where(o => !o.Deleted)
                    .Where(e => e.GUID.Equals(GUID))
                    .FirstOrDefault();
        }

        public static List<Message> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", int UserId = 0, int? ParentId = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Messages
                    .Include(e => e.User)
                    .Where(o => !o.Deleted)
                    .Where(o => o.ParentId == ParentId);

                if (UserId > 0)
                    query = query.Where(e => e.UserId == UserId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", int UserId = 0, int? ParentId = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Messages
                    .Where(o => !o.Deleted)
                    .Where(o => o.ParentId == ParentId);

                if (UserId > 0)
                    query = query.Where(e => e.UserId == UserId);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                return query.Count();
            }
        }

        public bool Save(bool SendNotification = false)
        {
            var success = false;

            var user = User; User = null;
            UserId = user?.Id ?? UserId;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;

            var original = new Message();
            if (ParentId.HasValue)
            {
                original = Find(ParentId.Value);
                original.Date = DateTime.UtcNow;
                original.ReplyCount += 1;
                original.Save();
            }

            if (User == null && ParentId.HasValue && !string.IsNullOrEmpty(EmailAddress) && SendNotification)
            {
                var template = Template.Find(Title: "Message Reply");
                template.Replace("Content", Content);
                template.Replace("Reply URL", $"{Config.WebURL}/about/thread?guid={original.GUID}");

                var thread = List(ParentId: ParentId.Value, Sort: "Id").Where(e => e.Id != Id).ToList();
                thread.Insert(0, original);
                var row = template.GetSectionTemplate("Thread");
                var rows = new StringBuilder();
                foreach(var message in thread)
                {
                    var html = row;
                    html = Template.Replace(html, "Name", message.User?.Title ?? Name);
                    html = Template.Replace(html, "Date", message.Date.ToString("d MMMM yyyy h:mmtt"));
                    html = Template.Replace(html, "Message", message.Content);
                    rows.AppendLine(html);
                }
                template.ReplaceSectionTemplate("Thread", rows.ToString());

                new Email
                {
                    Message = template.EmailHtml(),
                    RecipientEmail = EmailAddress,
                    RecipientName = Name,
                    SenderEmail = "noreply@agrishare.app",
                    Subject = $"Reply: {original.Title}"
                }.Send();
            }

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                StatusId = MessageStatus.Unread;
                Date = DateTime.UtcNow;
                GUID = Guid.NewGuid().ToString();
                ctx.Messages.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Messages.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Save();
        }

        public object ListJson()
        {
            return new
            {
                Id,
                Name = User?.Title ?? Name,
                Title,
                ReplyCount,
                Status,
                Date
            };
        }

        public object DetailJson()
        {
            return new
            {
                Id,
                ParentId,
                Name = User?.Title ?? Name,
                EmailAddress = User?.EmailAddress ?? EmailAddress,
                Telephone = User?.Telephone ?? Telephone,
                Title,
                Content,
                Status,
                DateCreated
            };
        }
    }
}
