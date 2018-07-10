using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using nHL.Web.Components;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Infrastructure
{
    public static class NHLocalizationExtensions
    {
        public static IServiceCollection AddNhibernateRequestLocalization(this IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton<NHStringLocalizerFactory, NHStringLocalizerFactory>());
            services.Add(ServiceDescriptor.Singleton<MissingStringLocalizerFactory, MissingStringLocalizerFactory>(q => new MissingStringLocalizerFactory(q.GetRequiredService<NHStringLocalizerFactory>(), q.GetRequiredService<NHStringLocalizerFactory>(), q.GetRequiredService<IMissingStringLocalizerLogger>())));
            services.Add(ServiceDescriptor.Singleton<IAsyncLocalizerFactory, MissingStringLocalizerFactory>(q => q.GetRequiredService<MissingStringLocalizerFactory>()));
            services.Add(ServiceDescriptor.Singleton<IStringLocalizerFactory, MissingStringLocalizerFactory>(q => q.GetRequiredService<MissingStringLocalizerFactory>()));
            services.Add(ServiceDescriptor.Singleton<IMissingStringLocalizerLogger, NHMissingStringLocalizerLogger>());
            services.AddScoped(typeof(IAsyncStringLocalizer<>), typeof(AsyncStringLocalizer<>));
            services.AddScoped(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            //should I use TryAddEnumerable?
            services.Add(ServiceDescriptor.Singleton<IConfigureOptions<RequestLocalizationOptions>, NHRequestLocalizationConfiguration>());
            return services;
        }
    }
}
