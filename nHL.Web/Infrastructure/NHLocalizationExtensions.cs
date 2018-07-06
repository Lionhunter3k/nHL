using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using nHL.Web.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Infrastructure
{
    public static class NHLocalizationExtensions
    {
        public static IServiceCollection AddNhibernateRequestLocalization(this IServiceCollection services)
        {
            services.AddLocalization()
                .Replace(ServiceDescriptor.Scoped<IStringLocalizerFactory, NHStringLocalizerFactory>())
                //should I use TryAddEnumerable?
                .Add(ServiceDescriptor.Singleton<IConfigureOptions<RequestLocalizationOptions>, NHRequestLocalizationConfiguration>());
            return services;
        }
    }
}
