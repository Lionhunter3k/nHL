using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using NHibernate;
using NHibernate.Linq;
using nHL.Domain;
using nHL.Web.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    using NHStringLocalizer = NHStringLocalizer<Dictionary<CultureInfo, Dictionary<string, LocalizedString>>, Dictionary<string, LocalizedString>>;

    public class NHStringLocalizerFactory : IStringLocalizerFactory, IAsyncLocalizerFactory
    {
        private readonly ISessionFactory sessionFactory;

        public NHStringLocalizerFactory(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        private NHStringLocalizer CreateLocalizer(string resource)
        {
            return new NHStringLocalizer(FindLocalizationsByResource, FindLocalizationsByResourceAsync, resource, new Dictionary<CultureInfo, Dictionary<string, LocalizedString>>());
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return CreateLocalizer(resourceSource.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return CreateLocalizer(string.Join(',', location, baseName));
        }

        private Dictionary<string, LocalizedString> FindLocalizationsByResource(string resource, CultureInfo culture)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            {
                return session.Query<LocalizedStringResource>()
                        .Where(q => q.Culture.Name == culture.Name)
                        .Where(q => q.Resource == resource)
                        .Select(q => new LocalizedString(q.Key, q.Text, false, q.Resource))
                        .ToDictionary(q => q.Name, q => q);
            }
        }

        private async Task<Dictionary<string, LocalizedString>> FindLocalizationsByResourceAsync(string resource, CultureInfo culture)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            {
                var localizedStrings = await session.Query<LocalizedStringResource>()
                    .Where(q => q.Culture.Name == culture.Name)
                    .Where(q => q.Resource == resource)
                    .Select(q => new LocalizedString(q.Key, q.Text, false, q.Resource))
                    .ToListAsync();
                var dictionary = new Dictionary<string, LocalizedString>(localizedStrings.Count);
                foreach (var localizedString in localizedStrings)
                {
                    dictionary.Add(localizedString.Name, localizedString);
                }
                return dictionary;
            }
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(Type resourceSource)
        {
            return CreateLocalizer(resourceSource.Name);
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(string baseName, string location)
        {
            return CreateLocalizer(string.Join(',', location, baseName));
        }
    }
}
