using Agrishare.Core.Entities;
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
            CleanDuplicateUsers();
        }

        private static void CleanDuplicateUsers()
        {
            Console.WriteLine("--Cleaning Users--");

            var list = Agrishare.Core.Entities.User.List();
            foreach (var user in list)
            {
                // check if user has been deleted
                using (var ctx = new AgrishareEntities())
                {
                    var existsSql = $"SELECT COUNT(Id) FROM users WHERE Id = {user.Id}";
                    bool exists = ctx.Database.SqlQuery<int>(existsSql).FirstOrDefault() == 1;
                    if (!exists)
                        continue;
                }

                // find duplicates
                var replaceIds = string.Empty;
                using (var ctx = new AgrishareEntities())
                {
                    var replaceIdsSql = $"SELECT GROUP_CONCAT(Id) FROM users WHERE telephone = \"{user.Telephone}\" AND Id <> {user.Id}";
                    replaceIds = ctx.Database.SqlQuery<string>(replaceIdsSql).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(replaceIds))
                {
                    var sql = $@"
                        UPDATE bookingcomments SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE bookings SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE bookingusers SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE counters SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE devices SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE journals SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE listings SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE messages SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE notifications SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE ratings SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        UPDATE uservouchers SET UserId = {user.Id} WHERE UserId IN ({replaceIds});
                        DELETE FROM users WHERE Id IN ({replaceIds});";

                    using (var ctx = new AgrishareEntities())
                    {
                        var updateCount = ctx.Database.ExecuteSqlCommand(sql);
                        Console.WriteLine($"\r{user.Title}: {updateCount} updates");
                    }
                }
                else
                {
                    // "\r" replaces the current line, needs .Write so we don't add a new line (\n)
                    Console.Write($"\rIgnoring {user.Title}...                                       ");
                }
            }

            Console.WriteLine($"--Processed {list.Count} users--");
        }

        private static void UpdateReferrals()
        {
            Console.WriteLine("--Updating Users--");

            var list = Agrishare.Core.Entities.User.List();
            foreach (var user in list)
            {
                if (user.ReferralCount > 0)
                {
                    Agrishare.Core.Entities.User.UpdateReferralCount(user.Id);
                    Console.WriteLine(user.Title);
                }
            }

            Console.WriteLine($"--Processed {list.Count} users--");
        }

        private static void UpdateDeviceSubscriptions()
        {
            Console.WriteLine("Processing device subscriptions");

            var list = Agrishare.Core.Entities.Device.List();
            foreach (var device in list)
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
