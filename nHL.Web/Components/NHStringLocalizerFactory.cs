using Microsoft.Extensions.Localization;
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
    public interface IMissingStringLocalizerLogger
    {
        Task FlushMissingStringsAsync();

        void LogMissingLocalization(LocalizedString localizedString);
    }

    public class NHStringLocalizerFactory : IStringLocalizerFactory, IMissingStringLocalizerLogger
    {
        private readonly ISession session;

        private readonly Dictionary<string, NHStringLocalizer> cachedResources = new Dictionary<string, NHStringLocalizer>();

        public NHStringLocalizerFactory(ISession session)
        {
            this.session = session;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (cachedResources.TryGetValue(resourceSource.Name, out NHStringLocalizer localizer))
                return localizer;
            var resources = FindLocalizationsByResource(resourceSource.Name);
            localizer = new NHStringLocalizer(resources, resourceSource.Name);
            cachedResources.Add(resourceSource.Name, localizer);
            return localizer;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var resource = baseName + "." + location;
            var resources = FindLocalizationsByResource(resource);
            return new NHStringLocalizer(resources, resource);
        }

        public async Task FlushMissingStringsAsync()
        {
            var shouldFlush = false;
            foreach(var missingResource in cachedResources.SelectMany(q => q.Value.Resources).Where(q => q.ResourceNotFound))
            {
                shouldFlush = true;
                session.Save(new LocalizedStringResource { Key = missingResource.Name, Resource = missingResource.SearchedLocation, Text = missingResource.Value });
            }
            if(shouldFlush)
                await session.FlushAsync();
        }

        private Dictionary<string, LocalizedString> FindLocalizationsByResource(string resource)
        {
            return session.Query<LocalizedStringResource>().Where(q => q.Resource == resource).ToList().ToDictionary(q => q.Key, q => new LocalizedString(q.Key, q.Text));
        }
    }

    public class NHStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, LocalizedString> resources;
        private readonly string resource;

        public NHStringLocalizer(Dictionary<string, LocalizedString> resources, string resource)
        {
            this.resources = resources;
            this.resource = resource;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if(resources.TryGetValue(name, out LocalizedString value))
                {
                    return value;
                }
                var notFoundResource = new LocalizedString(name, name, true, resource);

                resources.Add(name, notFoundResource);
                return notFoundResource;
            }
        }

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, string.Format(this[name], arguments));

        public IEnumerable<LocalizedString> Resources => resources.Values;

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
