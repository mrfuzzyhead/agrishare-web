using Agrishare.Core.Entities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class SMS
    {
        private static string Username => Config.Find(Key: "SMS Username").Value;
        private static string Sender => Config.Find(Key: "SMS Sender").Value;
        private static bool Live => Config.Find(Key: "SMS Live")?.Value == "True";
        private static bool Log => Config.Find(Key: "SMS Log")?.Value == "True";

        public static decimal GetBalance()
        {
            var client = new RestClient($"https://www.txt.co.zw/Remote/AccountBalance?Username={Username}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            var response = client.Execute(request);
            return Convert.ToDecimal(response.Content);
        }

        public static bool SendMessage(string Recipient, string Message)
        {
            if (!Live)
            {
                Entities.Log.Debug("SMS.SendMessage", $"To: {Recipient} Message: {Message}");
                return true;
            }

            try
            {
                var url = $"https://www.txt.co.zw/Remote/SendMessage?Username={Username}&Recipients={Recipient}&Body={Message}&sending_number={Sender}";
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Cache-Control", "no-cache");
                var response = client.Execute(request);

                if (Log)
                    Entities.Log.Debug("SMS.SendMessage", url + Environment.NewLine + JsonConvert.SerializeObject(response));

                if (response.Content.StartsWith("SUCCESS"))
                    return true;

                Entities.Log.Error("SMS.SendMessage", JsonConvert.SerializeObject(response));
                return false;
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SMS.SendMessage", ex);
                return false;
            }


        }

        public static bool SendMessages(List<string> Recipients, string Message)
        {
            var recipientList = String.Join(",", Recipients);

            if (!Live)
            {
                Entities.Log.Debug("SMS.SendMessages", $"To: {recipientList} Message: {Message}");
                return true;
            }

            try
            {
                var url = $"https://www.txt.co.zw/Remote/SendMessage";
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddParameter("undefined", $"username={Username}&Recipients={recipientList}&Body={Message}&sending_number={Sender}", ParameterType.RequestBody);

                var response = client.Execute(request);

                if (Log)
                    Entities.Log.Debug("SMS.SendMessages", url + Environment.NewLine + JsonConvert.SerializeObject(response));

                if (response.Content.StartsWith("SUCCESS"))
                    return true;

                Entities.Log.Error("SMS.SendMessages", JsonConvert.SerializeObject(response));
                return false;
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SMS.SendMessages", ex);
                return false;
            }


        }
    }
}
