﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache
{
    public class DistributedCache<T> : DistributedCache, ICache<T> where T : class
    {
        public DistributedCache(IDistributedCache client) : base(client) { }
    }
    public class DistributedCache : ICache
    {
        protected readonly IDistributedCache? _client;
        private const string _keyCollection = "___all_keys";

        public DistributedCache() { }

        public DistributedCache(IDistributedCache client)
        {
            _client = client;
        }

        public IEnumerable<string> Keys => Get<HashSet<string>>(_keyCollection) ?? new HashSet<string>();

        public object? Get(string key) => Get<object>(key);

        public T? Get<T>(string key)
        {
            try
            {
                var result = _client?.Get(key);
                if (result != null)
                {
                    return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result));
                }
            }
            catch
            {
                return default;
            }
            return default;
        }

        public void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public void Set(string key, object value, ICacheEntryOptions? options)
        {
            var _options = new DistributedCacheEntryOptions();
            if (options != null)
            {
                _options.AbsoluteExpiration = options.AbsoluteExpiration;
                _options.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                _options.SlidingExpiration = options.SlidingExpiration;
            }

            _client?.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), _options);

            if (key != _keyCollection && !Keys.Contains(key))
                SyncKeys(Keys.Append(key).ToHashSet<string>());

        }

        public void Remove(string key)
        {
            _remove(key); 

            if (Keys.Contains(key))
                SyncKeys(Keys.Where(_ => _ != key).ToHashSet<string>());
        }

        private void _remove(string key)
        {
            _client?.Remove(key); 
        }

        public void Clear()
        {
            foreach (var k in Keys)
                _remove(k);

            SyncKeys(new HashSet<string>());
        }

        private void SyncKeys(HashSet<string> keys)
        {
            Set(_keyCollection, keys, new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) });
        }
    }
}
