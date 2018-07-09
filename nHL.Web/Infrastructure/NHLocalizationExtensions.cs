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
            services.AddLocalization();
            services.Add(ServiceDescriptor.Scoped<NHStringLocalizerFactory, NHStringLocalizerFactory>());
            services.Add(ServiceDescriptor.Scoped<IMissingStringLocalizerLogger, NHStringLocalizerFactory>(q => q.GetRequiredService<NHStringLocalizerFactory>()));
            services.Add(ServiceDescriptor.Scoped<IAsyncLocalizerFactory, NHStringLocalizerFactory>(q => q.GetRequiredService<NHStringLocalizerFactory>()));
            services.Replace(ServiceDescriptor.Scoped<IStringLocalizerFactory, NHStringLocalizerFactory>(q => q.GetRequiredService<NHStringLocalizerFactory>()));
            services.TryAddTransient(typeof(IAsyncStringLocalizer<>), typeof(NHAsyncStringLocalizer<>));
            //should I use TryAddEnumerable?
            services.Add(ServiceDescriptor.Singleton<IConfigureOptions<RequestLocalizationOptions>, NHRequestLocalizationConfiguration>());
            return services;
        }
    }
}
