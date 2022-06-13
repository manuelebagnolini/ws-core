﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache
{
    public class CacheEntryOptions : ICacheEntryOptions
    {
        public CacheEntryOptions() { }

        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        public TimeSpan? SlidingExpiration { get; set; }

        public class Expiration
        {
            private static Options.Duration _duration { get; set; } = new Options.Duration() { };
            public Expiration(Options.Duration duration = null)
            {
                _duration = duration ?? new Options.Duration() { };
            }
            public static CacheEntryOptions Fast => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Fast) };
            public static CacheEntryOptions Medium => new () { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Medium) };
            public static CacheEntryOptions Slow => new () { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Slow) };
            public static CacheEntryOptions Never => new () { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Never) };

            /*
            public static void Set()
            {
                Fast = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Fast) };
                Medium = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Medium) };
                Slow = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Slow) };
                Never = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Never) };
            }*/
        }
    }
}
