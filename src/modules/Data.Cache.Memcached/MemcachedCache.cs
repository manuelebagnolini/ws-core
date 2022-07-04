﻿using System;
using System.Collections.Generic;
using Enyim.Caching;
using System.Linq;

namespace Ws.Core.Extensions.Data.Cache.Memcached
{
    public class MemcachedCache<T> : MemcachedCache, ICache<T> where T : class, new()  {
        public MemcachedCache(IMemcachedClient client): base(client) { }
    }
    public class MemcachedCache : ICache
    {
        protected readonly IMemcachedClient? _client;
        private const string _keyCollection = "___all_keys";
        public MemcachedCache() { }
        public MemcachedCache(IMemcachedClient? client)
        {
            _client = client;
        }

        public IEnumerable<string> Keys => Get<HashSet<string>>(_keyCollection) ?? new HashSet<string>();

        public object? Get(string key) => Get<object>(key);

        public T? Get<T>(string key) =>  _client != null ? _client.Get<T>(key) : default;

        public void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public void Set(string key, object value, ICacheEntryOptions? options)
        {
            DateTime ExpireAt = DateTime.Now.AddTicks((CacheEntryOptions.Expiration.Never?.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromDays(30)).Ticks);
            if (options != null)
            {
                if (options.AbsoluteExpiration != null)
                    ExpireAt = options.AbsoluteExpiration.Value.DateTime;
                else if (options.AbsoluteExpirationRelativeToNow != null)
                    ExpireAt = DateTime.Now.AddTicks(options.AbsoluteExpirationRelativeToNow.Value.Ticks);
                else if (options.SlidingExpiration != null)
                    ExpireAt = DateTime.Now.AddTicks(options.SlidingExpiration.Value.Ticks);
            }
            _client?.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value, ExpireAt);

            if (key != _keyCollection && !Keys.Contains(key))
                SyncKeys(Keys.Append(key).ToHashSet<string>());
        }

        public void Remove(string key)
        {
            _client?.Remove(key);

            if (Keys.Contains(key))
                SyncKeys(Keys.Where(_ => _ != key).ToHashSet<string>());
        }

        public void Clear()
        {
            _client?.FlushAll();
            SyncKeys(new HashSet<string>());
        }

        private void SyncKeys(HashSet<string> keys)
        {
            Set(_keyCollection, keys, new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) });
        }
    }
}
