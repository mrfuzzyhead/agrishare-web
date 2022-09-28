using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Processing device subscriptions");

            var list = Agrishare.Core.Entities.Device.List();
            foreach(var device in list)
            {
                if (string.IsNullOrEmpty(device.EndpointARN))
                {
                    Console.WriteLine($"{device.User.Title} - skipping - missing endpoint");
                    continue;
                }

                if (!string.IsNullOrEmpty(device.SubscriptionARN))
                {
                    Console.WriteLine($"{device.User.Title} - skipping - already subscribed");
                    continue;
                }
                
                Console.WriteLine($"{device.User.Title} - subscribing to topic...");
                if (SNS.SubscribeToBulkTopic(device.EndpointARN, out var arn))
                {
                    device.SubscriptionARN = arn;
                    device.Save();
                    Console.WriteLine($"OK");
                }
                else
                {
                    Console.WriteLine($"ERROR");
                }
            }

            Console.WriteLine($"--Processed {list.Count} devices--");
        }
    }
}
