using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using NHibernate;
using NHibernate.Linq;
using nHL.Domain;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    using NHStringLocalizer = NHStringLocalizer<Dictionary<CultureInfo, Dictionary<string, LocalizedString>>, Dictionary<string, LocalizedString>>;

    public class NHStringLocalizerFactory : IStringLocalizerFactory, IAsyncLocalizerFactory, IMissingStringLocalizerLogger, IDisposable
    {
        private readonly Lazy<IStatelessSession> session;

        private readonly Dictionary<string, NHStringLocalizer> cachedResources = new Dictionary<string, NHStringLocalizer>();

        private readonly ISessionFactory sessionFactory;

        public NHStringLocalizerFactory(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            this.session = new Lazy<IStatelessSession>(CreateLocalizationSession, false);
        }

        private IStatelessSession CreateLocalizationSession()
        {
            return sessionFactory.OpenStatelessSession();
        }

        private NHStringLocalizer GetOrCreateLocalizer(string resource)
        {
            if (cachedResources.TryGetValue(resource, out var localizer))
                return localizer;
            localizer = new NHStringLocalizer(FindLocalizationsByResource, FindLocalizationsByResourceAsync, resource);
            cachedResources.Add(resource, localizer);
            return localizer;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return GetOrCreateLocalizer(resourceSource.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return GetOrCreateLocalizer(string.Join('.', baseName, location));
        }

        private Dictionary<string, LocalizedString> FindLocalizationsByResource(string resource, CultureInfo culture)
        {
            return session.Value.Query<LocalizedStringResource>()
                .Where(q => q.Culture.Name == culture.Name)
                .Where(q => q.Resource == resource)
                .Select(q => new LocalizedString(q.Key, q.Text, false, q.Resource))
                .ToDictionary(q => q.Name, q => q);
        }

        private async Task<Dictionary<string, LocalizedString>> FindLocalizationsByResourceAsync(string resource, CultureInfo culture)
        {
            var localizedStrings = await session.Value.Query<LocalizedStringResource>()
                .Where(q => q.Culture.Name == culture.Name)
                .Where(q => q.Resource == resource)
                .Select(q => new LocalizedString(q.Key, q.Text, false, q.Resource))
                .ToListAsync();
            var dictionary = new Dictionary<string, LocalizedString>(localizedStrings.Count);
            foreach(var localizedString in localizedStrings)
            {
                dictionary.Add(localizedString.Name, localizedString);
            }
            return dictionary;
        }

        public void Dispose()
        {
            if (session.IsValueCreated)
                session.Value.Dispose();
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(Type resourceSource)
        {
            return GetOrCreateLocalizer(resourceSource.Name);
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(string baseName, string location)
        {
            return GetOrCreateLocalizer(string.Join('.', baseName, location));
        }

        async Task IMissingStringLocalizerLogger.FlushMissingStringsAsync()
        {
            foreach(var resource in cachedResources)
            {
                foreach(var culture in resource.Value.LocalizedStrings)
                {
                    var persistedCulture = session.Value.Get<Culture>(culture.Key);
                    if(persistedCulture != null)
                    {
                        foreach (var missingLocalizedString in culture.Value.Values.Where(q => q.ResourceNotFound))
                        {
                            await session.Value.InsertAsync(new LocalizedStringResource { Culture = persistedCulture, Key = missingLocalizedString.Name, Resource = resource.Key, Text = missingLocalizedString.Value });
                        }
                    }
                }
            }
        }

        void IMissingStringLocalizerLogger.LogMissingLocalization(string location, string name, CultureInfo culture)
        {
            //noop
        }
    }
}
