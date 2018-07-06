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
    public class NHStringLocalizer : IStringLocalizer
    {
        private readonly Func<string, CultureInfo, Dictionary<string, LocalizedString>> localizedStringsRetriever;
        private readonly string resource;
        private readonly CultureInfo culture;
        private readonly Dictionary<CultureInfo, Dictionary<string, LocalizedString>> localizedStrings;

        public NHStringLocalizer(Func<string, CultureInfo, Dictionary<string, LocalizedString>> localizedStringsRetriever,
            string resource,
            CultureInfo culture,
            Dictionary<CultureInfo, Dictionary<string, LocalizedString>> localizedStrings)
        {
            this.localizedStringsRetriever = localizedStringsRetriever;
            this.resource = resource;
            this.culture = culture;
            this.localizedStrings = localizedStrings;
        }

        public NHStringLocalizer(Func<string, CultureInfo, Dictionary<string, LocalizedString>> localizedStringsRetriever,
          string resource)
        {
            this.localizedStringsRetriever = localizedStringsRetriever;
            this.resource = resource;
        }

        private Dictionary<string, LocalizedString> RetrieveLocalizedStrings()
        {
            var currentCulture = GetCurrentCulture();
            if (localizedStrings.TryGetValue(currentCulture, out var localizedStringsForCurrentCulture))
                return localizedStringsForCurrentCulture;
            localizedStringsForCurrentCulture = localizedStringsRetriever(resource, currentCulture);
            localizedStrings.Add(currentCulture, localizedStringsForCurrentCulture);
            return localizedStringsForCurrentCulture;
        }

        private CultureInfo GetCurrentCulture()
        {
            return culture ?? CultureInfo.CurrentCulture;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var localizedStringsForCurrentCulture = RetrieveLocalizedStrings();
                if (localizedStringsForCurrentCulture.TryGetValue(name, out LocalizedString value))
                {
                    return value;
                }
                var notFoundResource = new LocalizedString(name, name, true, resource);
                localizedStringsForCurrentCulture.Add(name, notFoundResource);
                return notFoundResource;
            }
        }

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, string.Format(this[name], arguments));

        public IEnumerable<LocalizedString> Resources => RetrieveLocalizedStrings().Values;

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new NHStringLocalizer(localizedStringsRetriever, resource, culture, localizedStrings);
        }
    }
}
