using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using NHibernate;
using nHL.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    public class NHStringLocalizerFactory : IStringLocalizerFactory, IDisposable
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
            if (cachedResources.TryGetValue(resource, out NHStringLocalizer localizer))
                return localizer;
            localizer = new NHStringLocalizer(FindLocalizationsByResource, resource);
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
                .ToDictionary(q => q.Key, q => new LocalizedString(q.Key, q.Text));
        }

        public void Dispose()
        {
            if (session.IsValueCreated)
                session.Value.Dispose();
        }
    }
}
