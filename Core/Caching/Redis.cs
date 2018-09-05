using Agrishare.Core.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Caching
{
    class Redis
    {
        private static string connectionString = string.Empty;
        private static string ConnectionString
        {
            get
            {
                if (connectionString.IsEmpty())
                    connectionString = Config.Find(Key: "Redis Connection", IgnoreCache: true).Value;
                return connectionString;
            }
        }
        private static Lazy<ConnectionMultiplexer> connection;
        private static ConnectionMultiplexer RedisConnection() => connection.Value;

        static Redis()
        {
            connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConnectionString));
        }

        public static bool Add(string Key, string Value)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.StringSet(Key, Value);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.Add", ex);
                throw;
            }
        }

        public static string Get(string Key)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.StringGet(Key);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.Get", ex);
                throw;
            }
        }

        public static bool Remove(string Key)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.KeyDelete(Key);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.Remove", ex);
                throw;
            }
        }

        public static void SetHash(string Key, HashEntry[] Entries)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                db.HashSet(Key, Entries);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.SetHash", ex);
                throw;
            }
        }

        public static HashEntry[] GetHash(string Key)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.HashGetAll(Key);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.GetHash", ex);
                throw;
            }
        }

        public static bool RemoveHash(string Key)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.KeyDelete(Key);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.RemoveHash", ex);
                throw;
            }
        }

        public static bool SetHashEntry(string Key, string Field, string Value)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.HashSet(Key, Field, Value);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.SetHashEntry", ex);
                throw;
            }
        }

        public static string GetHashEntry(string Key, string Field)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.HashGet(Key, Field);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.GetHashEntry", ex);
                throw;
            }
        }

        public static bool RemoveHashEntry(string Key, string Field)
        {
            if (Key.IsEmpty())
                throw new ArgumentException("Key cannot be null or empty");

            try
            {
                var db = RedisConnection().GetDatabase();
                return db.HashDelete(Key, Field);
            }
            catch (Exception ex)
            {
                Log.Error("Redis.RemoveHashEntry", ex);
                throw;
            }
        }

        public static void Clear()
        {
            try
            {
                var options = ConfigurationOptions.Parse(ConnectionString);
                options.AllowAdmin = true;
                var conn = ConnectionMultiplexer.Connect(options);
                System.Net.EndPoint[] endpoints = conn.GetEndPoints();
                if (endpoints.Length > 0)
                {
                    var server = conn.GetServer(endpoints[0]);
                    server.FlushDatabase();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Redis.Empty", ex);
            }
        }
    }
}
