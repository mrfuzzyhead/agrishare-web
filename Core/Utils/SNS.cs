﻿/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class SNS
    {
        private static string ApplicationARN => Entities.Config.Find(Key: "AWS Application ARN").Value;
        private static string BulkTopicARN => Entities.Config.Find(Key: "AWS Bulk Topic ARN").Value;
        private static string AccessKey => Entities.Config.Find(Key: "AWS Access Key").Value;
        private static string SecretKey => Entities.Config.Find(Key: "AWS Secret Key").Value;
        private static RegionEndpoint Region => RegionEndpoint.GetBySystemName(Entities.Config.Find(Key: "AWS Region").Value);
        private static bool Log => Entities.Config.Find(Key: "Log SNS").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        private static IAmazonSimpleNotificationService CreateClient()
        {
            return AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(AccessKey, SecretKey, Region);
        }

        public static bool AddDevice(string DeviceToken, out string DeviceARN)
        {
            DeviceARN = null;
            try
            {
                using (var client = CreateClient())
                {
                    var request = new CreatePlatformEndpointRequest
                    {
                        PlatformApplicationArn = ApplicationARN,
                        Token = DeviceToken
                    };

                    var response = client.CreatePlatformEndpoint(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        DeviceARN = response.EndpointArn;

                        var attributeRequest = new GetEndpointAttributesRequest()
                        {
                            EndpointArn = DeviceARN
                        };
                        var attributeResponse = client.GetEndpointAttributes(attributeRequest);
                        if (attributeResponse.Attributes["Token"] != DeviceARN || attributeResponse.Attributes["Enabled"].ToLower() != "true")
                        {
                            var updateRequest = new SetEndpointAttributesRequest()
                            {
                                EndpointArn = DeviceARN,
                                Attributes = new Dictionary<string, string>()
                                {
                                    {"Token", DeviceToken },
                                    {"Enabled", "true" }
                                }
                            };

                            var updateResponse = client.SetEndpointAttributes(updateRequest);
                            return updateResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
                        }
                        else return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SNS.CreateDeviceEndpoint", ex);
                return false;
            }
        }

        public static bool RemoveDevice(string DeviceARN)
        {
            try
            {
                using (var client = CreateClient())
                {
                    var request = new DeleteEndpointRequest()
                    {
                        EndpointArn = DeviceARN
                    };

                    var resp = client.DeleteEndpoint(request);
                    return resp.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SNS.RemoveDeviceEndpoint", ex);
                return false;
            }
        }

        public static bool SendMessage(string DeviceARN, string Message, string Category = "", Dictionary<string, object> Params = null)
        {
            dynamic gcmPayload = new ExpandoObject();
            gcmPayload.data = new ExpandoObject();
            gcmPayload.data.message = Message;
            gcmPayload.data.category = Category;

            if (Params != null)
                foreach (string key in Params.Keys)
                    ((IDictionary<String, Object>)gcmPayload.data)[key] = Params[key];

            var payload = string.Format(@"{{""default"":{0},""GCM_SANDBOX"":{1},""GCM"":{1}}}",
               JsonConvert.SerializeObject(Message),
               JsonConvert.SerializeObject(JsonConvert.SerializeObject(gcmPayload)));

            if (Log)
                Entities.Log.Debug("SNS.SendMessage", JsonConvert.SerializeObject(payload));

            using (var client = CreateClient())
            {
                var request = new PublishRequest
                {
                    TargetArn = DeviceARN,
                    Message = payload,
                    Subject = Message,
                    MessageStructure = "json"
                };

                try
                {
                    var resp = client.Publish(request);
                    return (resp.HttpStatusCode == System.Net.HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    Entities.Log.Error("SNS.SendMessage", ex);
                    return false;
                }
            }
        }

        public static bool SubscribeToBulkTopic(string DeviceARN, out string SubscriptionARN)
        {
            var request = new SubscribeRequest
            {
                TopicArn = BulkTopicARN,
                Endpoint = DeviceARN,
                Protocol = "application"
            };

            try
            {
                var resp = CreateClient().Subscribe(request);
                var success = resp.HttpStatusCode == System.Net.HttpStatusCode.OK;
                if (success)
                {
                    SubscriptionARN = resp.SubscriptionArn;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("PushNotification.SubscribeToTopic", ex);
            }

            SubscriptionARN = null;
            return false;
        }

        public static bool UnsubscribeFromTopic(string SubscriptionARN)
        {
            try
            {
                var request = new UnsubscribeRequest()
                {
                    SubscriptionArn = SubscriptionARN
                };

                var resp = CreateClient().Unsubscribe(request);
                return resp.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Entities.Log.Error("PushNotification.UnsubscribeFromTopic", ex);
                return false;
            }
        }

        public static bool SendBulkMessage(MessageModel Model, out string ErrorMessage)
        {
            var success = false;
            ErrorMessage = string.Empty;

            dynamic apnsPayload = new Dictionary<string, object>();
            apnsPayload["aps"] = new
            {
                alert = new Dictionary<string, object>
                {
                    { "title", Model.Title },
                    { "body", Model.Message },
                },
                sound = "default",
                category = Model.Category
            };

            dynamic gcmPayload = new ExpandoObject();
            gcmPayload.data = new ExpandoObject();
            gcmPayload.data.title = Model.Title;
            gcmPayload.data.message = Model.Message;
            gcmPayload.data.category = Model.Category;

            if (Model.Params != null)
                foreach (var item in Model.Params)
                {
                    ((IDictionary<string, object>)gcmPayload.data)[item.Key] = item.Value;
                    ((IDictionary<string, object>)apnsPayload)[item.Key] = item.Value.ToString();
                }

            var payload = string.Format(@"{{""default"":{0},""APNS_SANDBOX"":{1},""APNS"":{1},""GCM_SANDBOX"":{2},""GCM"":{2}}}",
              JsonConvert.SerializeObject(Model.Message),
              JsonConvert.SerializeObject(JsonConvert.SerializeObject(apnsPayload)),
              JsonConvert.SerializeObject(JsonConvert.SerializeObject(gcmPayload)));

            if (Log)
                Entities.Log.Debug("PushNotification.SendMessage", JsonConvert.SerializeObject(payload));

            var request = new PublishRequest
            {
                TopicArn = BulkTopicARN,
                Message = payload,
                MessageStructure = "json"
            };

            try
            {
                var resp = CreateClient().Publish(request);
                success = resp.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Entities.Log.Error("PushNotification.SendBulkMessage", ex);
            }

            return success;
        }

        public class MessageModel
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string Category { get; set; }
            public Dictionary<string, object> Params { get; set; }
        }

    }
}
