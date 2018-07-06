using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using NHibernate;
using nHL.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace nHL.Web.Infrastructure
{
    public class NHRequestLocalizationConfiguration : IConfigureOptions<RequestLocalizationOptions>
    {
        private readonly ISessionFactory sessionFactory;

        public NHRequestLocalizationConfiguration(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public void Configure(RequestLocalizationOptions options)
        {
            options.DefaultRequestCulture = new RequestCulture("en-US");
            using (var session = sessionFactory.OpenStatelessSession())
            {
                var enabledCultures = new HashSet<string>(session.Query<Culture>().Where(q => q.Disabled == false).Select(q => q.Name));
                var availableLocalCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(q => enabledCultures.Contains(q.Name)).ToArray();
                options.SupportedCultures = availableLocalCultures;
                options.SupportedUICultures = availableLocalCultures;
            }
        }
    }
}
