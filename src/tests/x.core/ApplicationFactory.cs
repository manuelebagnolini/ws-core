﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ws.Core.Extensions.Data;

namespace x.core;

interface IWebApplicationFactoryEnvironment
{
    string Environment { get; }
}

public class LocalApplicationFactory : WebApplicationFactory<Program>, IWebApplicationFactoryEnvironment
{
    public string Environment => "Local";
    protected override TestServer CreateServer(IWebHostBuilder builder) {
        builder.UseEnvironment(Environment);
        return base.CreateServer(builder);
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environment);        
        return base.CreateHost(builder);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environment);
        builder.UseConfiguration(new ConfigurationBuilder().AddJsonFile($"ext-settings.{Environment}.json").Build());
        base.ConfigureWebHost(builder);
    }
}
public class MockApplicationFactory : WebApplicationFactory<Program>, IWebApplicationFactoryEnvironment
{
    public string Environment => "Mock";

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environment);
        return base.CreateServer(builder);
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environment);
        // override test services
        builder.ConfigureServices(_ =>
        {
            
            _
                .RemoveAll(typeof(IRepository<,>))
                .RemoveAll<Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService>()
                ;
            

            // db repo
            _
                //.AddTransient(typeof(Ws.Core.Extensions.Data.Repository.InMemory<,>), typeof(Ws.Core.Extensions.Data.Repository.InMemory<,>))
                //.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<x.core.Models.Agenda,string>), typeof(Ws.Core.Extensions.Data.Repository.InMemory<x.core.Models.Agenda,string>))
                .AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<,>), typeof(Ws.Core.Extensions.Data.Repository.InMemory<,>))
                //.AddTransient(typeof(Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService), typeof(x.core.HealthCheckAppLogService<Ws.Core.Extensions.Data.Repository.InMemory<Log,int>>))
                ;
        });        
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environment);
        //builder.UseConfiguration(new ConfigurationBuilder().AddJsonFile($"ext-settings.{Environment}.json").Build());
        base.ConfigureWebHost(builder);
    }
}

public enum WebApplicationFactoryType
{
    Development,
    Mock,
    Local
}