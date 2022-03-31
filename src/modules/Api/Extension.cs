﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;

namespace Ws.Core.Extensions.Api
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();
        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {            
            base.Execute(builder, serviceProvider);

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var _session = _options.Session;
            if (_session != null)
            {
                builder.Services.AddSession(opt =>
                {
                    var _cookie = _session.Cookie;
                    if (_cookie != null)
                    {
                        opt.Cookie = _cookie;
                    }
                    opt.IdleTimeout = TimeSpan.FromMinutes(_session.IdleTimeoutInMinutes);
                });
            }

            builder.Services
                .AddControllers()                
                .AddNewtonsoftJson(opt =>
                {
                    var _setting = opt.SerializerSettings;
                    _options.Serialization.FromJsonSerializerSettings(ref _setting);
                }
                )
                /*
                .AddJsonOptions(opt =>
                {
                    var _setting = opt.JsonSerializerOptions;
                    _options.Serialization.FromJsonSerializerSettings(ref _setting);
                })
                */
            ;                           

            var _doc = _options.Documentation;
            if (_doc != null)
            {
                builder.Services
                    .AddEndpointsApiExplorer()
                    .AddSwaggerGen(opt =>
                {
                    opt.CustomSchemaIds(_ => _.FullName);

                    opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    //Xml comments
                    if (_doc.XmlComments != null && !string.IsNullOrEmpty(_doc.XmlComments.FileName))
                    {
                        var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, _doc.XmlComments.FileName);
                        opt.IncludeXmlComments(filePath, includeControllerXmlComments: _doc.XmlComments.IncludeControllerComments);
                    }

                    foreach (var doc in _doc.Endpoints?.Select((e, i) => new { e, i }))
                    {
                        var _id = string.IsNullOrEmpty(doc.e?.Id) ? $"v{doc.i + 1}" : doc.e?.Id;
                        opt.SwaggerDoc(
                            _id,
                            new Microsoft.OpenApi.Models.OpenApiInfo()
                            {
                                Title = string.IsNullOrEmpty(doc.e?.Title) ? $"API v{doc.i + 1}" : doc.e?.Title,
                                Version = doc.e?.Version ?? _id
                            }
                            );
                    }

                    if (_doc.SecurityDefinitions != null)
                    {
                        //opt.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                        if (_doc.SecurityDefinitions.Bearer)
                        {
                            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                            {
                                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                                In = ParameterLocation.Header,
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey,
                                Scheme = "Bearer",
                                BearerFormat = "JWT"
                            });
                            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                                    },
                                    Array.Empty<string>()
                                }
                            });
                        }

                        if (_doc.SecurityDefinitions.Cookies != null && _doc.SecurityDefinitions.Cookies.Any())
                        {
                            foreach (var cookieName in _doc.SecurityDefinitions.Cookies)
                            {
                                opt.AddSecurityDefinition(cookieName, new OpenApiSecurityScheme
                                {
                                    Description = $"Cookie Auth: {cookieName}",
                                    In = ParameterLocation.Cookie,
                                    Name = cookieName,
                                    Type = SecuritySchemeType.ApiKey
                                });
                                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = cookieName }
                                    },
                                    Array.Empty<string>()
                                }
                            });
                            }
                        }

                    }

                });
            }
        }

        public override void Execute(WebApplication app)
        {            
            base.Execute(app);

            app.MapControllers();

            if (_options.Session != null)
                app.UseSession();           
            
            var _doc = _options.Documentation;

            if (_doc != null)
            {
                app.UseSwagger(opt =>
                {
                    opt.RouteTemplate = _doc.RoutePrefix + "/{documentName}/swagger.json";
                    /*opt.PreSerializeFilters.Add((doc, rq) =>
                    {
                        doc.Host = rq.Host.Value;
                    });*/
                });                

                app.UseSwaggerUI(opt =>
                {
                    opt.RoutePrefix = _doc.RoutePrefix;
                    opt.EnableDeepLinking();
                    opt.DisplayRequestDuration();
                    if (_doc.Ui != null)
                    {
                        var ui = _doc.Ui;
                        if (!string.IsNullOrEmpty(ui.InjectJs))
                            opt.InjectJavascript(ui.InjectJs);
                        if (!string.IsNullOrEmpty(ui.InjectCss))
                            opt.InjectStylesheet(ui.InjectCss);
                    }
                    foreach (var doc in _doc.Endpoints?.Select((e, i) => new { e, i }))
                    {
                        var _id = string.IsNullOrEmpty(doc.e.Id) ? $"v{doc.i + 1}" : doc.e.Id;
                        opt.SwaggerEndpoint($"/{_doc.RoutePrefix}/{_id}/swagger.json", string.IsNullOrEmpty(doc.e.Title) ? $"API v{doc.i + 1}" : doc.e.Title);
                    }

                });
            }
        }
    }

    public class Documentation { }
}
