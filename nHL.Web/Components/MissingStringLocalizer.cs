using Microsoft.Extensions.Localization;
using nHL.Web.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    public class MissingStringLocalizer : IStringLocalizer, IAsyncStringLocalizer, IReadOnlyDictionary<string, GetOrFormatLocalizedString>
    {
        private readonly IStringLocalizer inner;
        private readonly IAsyncStringLocalizer asyncInner;
        private readonly IMissingStringLocalizerLogger missing;
        private readonly CultureInfo culture;
        private IReadOnlyDictionary<string, GetOrFormatLocalizedString> asyncLocalizedDictionary;

        IEnumerable<string> IReadOnlyDictionary<string, GetOrFormatLocalizedString>.Keys => asyncLocalizedDictionary.Keys;

        IEnumerable<GetOrFormatLocalizedString> IReadOnlyDictionary<string, GetOrFormatLocalizedString>.Values => asyncLocalizedDictionary.Values;

        int IReadOnlyCollection<KeyValuePair<string, GetOrFormatLocalizedString>>.Count => asyncLocalizedDictionary.Count;

        GetOrFormatLocalizedString IReadOnlyDictionary<string, GetOrFormatLocalizedString>.this[string key]
        {
            get
            {
                var localizedString = asyncLocalizedDictionary[key];
                if (localizedString.LocalizedString.ResourceNotFound)
                    missing.LogMissingLocalization(localizedString.LocalizedString.SearchedLocation, localizedString.LocalizedString.Name, culture ?? CultureInfo.CurrentCulture);
                return localizedString;
            }
        }

        public MissingStringLocalizer(IStringLocalizer inner, IMissingStringLocalizerLogger missing)
        {
            this.inner = inner;
            this.missing = missing;
        }

        public MissingStringLocalizer(IStringLocalizer inner, IMissingStringLocalizerLogger missing, CultureInfo culture)
        {
            this.inner = inner;
            this.missing = missing;
            this.culture = culture;
        }

        public MissingStringLocalizer(IAsyncStringLocalizer asyncInner, IMissingStringLocalizerLogger missing)
        {
            this.asyncInner = asyncInner;
            this.missing = missing;
        }

        public MissingStringLocalizer(IAsyncStringLocalizer asyncInner, IMissingStringLocalizerLogger missing, CultureInfo culture)
        {
            this.asyncInner = asyncInner;
            this.missing = missing;
            this.culture = culture;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var localizedString = inner[name];
                if (localizedString.ResourceNotFound)
                    missing.LogMissingLocalization(localizedString.SearchedLocation, localizedString.Name, culture ?? CultureInfo.CurrentCulture);
                return localizedString;
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizedString = inner[name, arguments];
                if (localizedString.ResourceNotFound)
                    missing.LogMissingLocalization(localizedString.SearchedLocation, localizedString.Name, culture ?? CultureInfo.CurrentCulture);
                return localizedString;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return inner.GetAllStrings(includeParentCultures);
        }

        public async ValueTask<IReadOnlyDictionary<string, GetOrFormatLocalizedString>> GetLocalizedStringsAsync()
        {
            this.asyncLocalizedDictionary = await asyncInner.GetLocalizedStringsAsync();
            return this;
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new MissingStringLocalizer(inner.WithCulture(culture), missing, culture);
        }

        bool IReadOnlyDictionary<string, GetOrFormatLocalizedString>.ContainsKey(string key)
        {
            return asyncLocalizedDictionary.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, GetOrFormatLocalizedString>.TryGetValue(string key, out GetOrFormatLocalizedString value)
        {
            return asyncLocalizedDictionary.TryGetValue(key, out value);
        }

        IEnumerator<KeyValuePair<string, GetOrFormatLocalizedString>> IEnumerable<KeyValuePair<string, GetOrFormatLocalizedString>>.GetEnumerator()
        {
            return asyncLocalizedDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return asyncLocalizedDictionary.GetEnumerator();
        }
    }
}
