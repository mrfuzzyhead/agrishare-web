using Agrishare.Core.Entities;
using Infobip.Api.Client;
using Infobip.Api.Config;
using Infobip.Api.Model;
using Infobip.Api.Model.Sms.Mt.Send;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Message = Infobip.Api.Model.Sms.Mt.Send.Message;

namespace Agrishare.Core.Utils
{
    public class SMS
    {
        private static bool Live => Config.Find(Key: "SMS Live")?.Value == "True";
        private static bool Log => Config.Find(Key: "SMS Log")?.Value == "True";
        private static string Sender => Config.Find(Key: "SMS Sender").Value;

        private static ApiKeyAuthConfiguration AuthConfig(Region Region)
        {
            var baseUrl = Config.Find(Key: $"Infobip Base URL ({Region.Title})")?.Value ?? "";
            var apiKey = Config.Find(Key: $"Infobip API Key ({Region.Title})")?.Value ?? "";
            return new ApiKeyAuthConfiguration(baseUrl, apiKey);
        }

        public static decimal GetBalance(Region Region)
        {
            var client = new GetAccountBalance(AuthConfig(Region));
            var task = Task.Run(() => client.ExecuteAsync()).Result;
            return task.Balance ?? 0M;
        }

        public static bool SendMessage(string RecipientNumber, string MessageText, Region Region)
        {
            if (!Live)
            {
                Entities.Log.Debug("SMS.SendMessage", $"To: {RecipientNumber} Message: {MessageText}");
                return true;
            }

            if (string.IsNullOrEmpty(MessageText))
            {
                Entities.Log.Error("SMS.SendMessage", "Empty message");
                return false;
            }

            var client = new SendMultipleTextualSmsAdvanced(AuthConfig(Region));
            var destination = new Destination
            {
                To = FormatNumber(RecipientNumber, Region)
            };
            var message = new Message
            {
                From = Sender,
                Destinations = new List<Destination>(1) { destination },
                Text = MessageText
            };
            var request = new SMSAdvancedTextualRequest
            {
                Messages = new List<Message>(1) { message }
            };

            var response = Task.Run(() => client.ExecuteAsync(request)).Result;
            var sentMessageInfo = response.Messages[0];

            if (Log)
                Entities.Log.Debug("SMS.SendMessage", JsonConvert.SerializeObject(sentMessageInfo));

            return sentMessageInfo.Status.GroupId == 1 || sentMessageInfo.Status.GroupId == 3;
        }

        public static bool SendMessages(List<string> RecipientList, string MessageText, Region Region)
        {
            if (!Live)
            {
                Entities.Log.Debug("SMS.SendMessages", $"To: {string.Join(", ", RecipientList)} Message: {MessageText}");
                return true;
            }

            var client = new SendMultipleTextualSmsAdvanced(AuthConfig(Region));

            var destinations = new List<Destination>();
            foreach(var number in RecipientList)
                destinations.Add(new Destination { To = FormatNumber(number, Region) });
            
            var message = new Message
            {
                From = Sender,
                Destinations = destinations,
                Text = MessageText
            };
            var request = new SMSAdvancedTextualRequest
            {
                Messages = new List<Message>(1) { message }
            };

            var response = Task.Run(() => client.ExecuteAsync(request)).Result;
            var sentMessageInfo = response.Messages[0];

            if (Log)
                Entities.Log.Debug("SMS.SendMessage", JsonConvert.SerializeObject(sentMessageInfo));

            return sentMessageInfo.Status.GroupId == 1 || sentMessageInfo.Status.GroupId == 3;
        }

        private static string FormatNumber(string Number, Region Region)
        {
            if (Number.Count() != 10)
                return string.Empty;

            Number = Regex.Replace(Number, "^0", "");

            switch (Region.Title)
            {
                case "Zimbabwe":
                    return "+263" + Number;
                case "Uganda":
                    return "+256" + Number;
                case "Malawi":
                    return "+265" + Number;
            }
            return string.Empty;
        }
    }
}
