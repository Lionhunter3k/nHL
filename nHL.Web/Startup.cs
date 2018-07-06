﻿using Autofac;
using CoreMusicStore.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using nHL.Web.Helpers;
using nHL.Web.Infrastructure;
using nHL.Web.Infrastructure.Persistence;
using System;
using System.IO;

namespace nHL.Web
{
    public class Startup : StartupBase
    {
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.AddAutofacProvider(builder =>
            {
                //nhibernate
                var nhibernateModule = new XmlNhibernateModule { SchemaRootPath = Environment.ContentRootPath, XmlCfgFileName = "hibernate.cfg.mssql.xml" };
                builder.RegisterModule(nhibernateModule);

                //services
                var servicesModule = new ServiceModule().AddAssemblyFor<Startup>();
                builder.RegisterModule(servicesModule);
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddRouting();

            services.AddNhibernateRequestLocalization();

            var mvcServiceBuilder = services
                .AddMvcCore()
                .AddApplicationPart<InputTagHelper>()
                .AddApplicationPart<UrlResolutionTagHelper>()
                .AddViews()
                .AddRazorViewEngine(options =>
                {
                    options.AddReferencesFromRuntimeContext();
                })
                .AddDataAnnotations()
                .AddFormatterMappings()
                .AddJsonFormatters(settings =>
                {
                    settings.ContractResolver = new DefaultContractResolver();
                })
                .AddAuthorization()//this also calls services.AddAuthorization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                // Add support for localizing strings in data annotations (e.g. validation messages) via the
                // IStringLocalizer abstractions.
                .AddDataAnnotationsLocalization();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  // Cookie settings
                  options.Cookie.HttpOnly = true;
                  options.Cookie.Expiration = TimeSpan.FromDays(150);
                  options.LoginPath = "/Account/LogOn"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                  options.LogoutPath = "/Account/LogOff"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                  // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                  options.AccessDeniedPath = "/Account/LogOn";
                  options.SlidingExpiration = true;
              });
            //uncomment this line if we have a singleton which might be called from a HTTP request
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddWebOptimizer(Environment,
                new CssBundlingSettings
                {
#if DEBUG
                    Minify = false
#else
                    Minify = true
#endif
                },
                new CodeBundlingSettings
                {
#if DEBUG
                    Minify = false
#else
                    Minify = true
#endif
                });
        }

        public override void Configure(IApplicationBuilder app)
        {
            //if we have an uncaught error, we should display the detailed error page if the request is local
            app.MapWhen(context => context.Request.IsLocal(), localApp =>
            {
                localApp.UseDeveloperExceptionPage();

                ConfigureApp(localApp);
            });

            //otherwise, we should show a friendly error page that doesn't try to load too much stuff that might be broken
            app.MapWhen(context => !context.Request.IsLocal(), remoteApp =>
            {
                remoteApp.UseExceptionHandler(exceptionApp =>
                {
                    exceptionApp.Use((context, next) =>
                    {
                        context.Request.Path = new PathString("/error.html");
                        return next();
                    });

                    exceptionApp.UseStaticFiles();
                });

                ConfigureApp(remoteApp);
            });
        }

        public void ConfigureApp(IApplicationBuilder app)
        {
            var contentFileProvider = new PhysicalFileProvider(
                Path.Combine(Environment.ContentRootPath, "Content"));

            var scriptsFileProvider = new PhysicalFileProvider(
                Path.Combine(Environment.ContentRootPath, "Scripts"));

            app.UseWebOptimizer(Environment, new WebOptimizer.FileProviderOptions[]
            {
                new WebOptimizer.FileProviderOptions { FileProvider = contentFileProvider, RequestPath = "/Content" },
                new WebOptimizer.FileProviderOptions { FileProvider = scriptsFileProvider, RequestPath = "/Scripts" },
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = contentFileProvider,
                RequestPath = "/Content"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = scriptsFileProvider,
                RequestPath = "/Scripts"
            });

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(locOptions);

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}