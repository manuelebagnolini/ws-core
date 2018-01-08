﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core.Extensions.Data.Cache.Memcached
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            if (_options.Client != null)
            {
                // default entry expiration
                if (Options.EntryExpirationInMinutes == null)
                    Options.EntryExpirationInMinutes = new core.Extensions.Data.Cache.Options.Duration();

                // init/override default cache profile
                //CacheEntryOptions.Expiration.Set();

                // service                
                serviceCollection.AddEnyimMemcached(_config.GetSection($"Configuration:Assemblies:{this.AssemblyName}:Options:Client"));                
                //serviceCollection.AddEnyimMemcached(_ => _.AddServer(_options.Client.Servers[0].Address, _options.Client.Servers[0].Port));                

                //DI
                serviceCollection.AddSingleton(typeof(ICache), typeof(MemcachedCache));                
                serviceCollection.TryAddTransient(typeof(ICacheRepository<>), typeof(Repository.CachedRepository<>));
            }
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            if (_options.Client != null)
                applicationBuilder.UseEnyimMemcached();
        }
    }
}