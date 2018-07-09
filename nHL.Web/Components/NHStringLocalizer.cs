using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using NHibernate;
using nHL.Domain;
using nHL.Web.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    public class NHStringLocalizer<TCultureDictionary, TStringDictionary> : IStringLocalizer, IAsyncStringLocalizer, IReadOnlyDictionary<string, GetOrFormatLocalizedString>
        where TCultureDictionary : IDictionary<CultureInfo, TStringDictionary>
        where TStringDictionary: IDictionary<string, LocalizedString>
    {
        private readonly Func<string, CultureInfo, TStringDictionary> localizedStringsRetriever;
        private readonly Func<string, CultureInfo, Task<TStringDictionary>> asyncLocalizedStringsRetriever;
        private readonly string resource;
        private readonly CultureInfo culture;

        public TCultureDictionary LocalizedStrings { get; }

        public NHStringLocalizer(Func<string, CultureInfo, TStringDictionary> localizedStringsRetriever,
            Func<string, CultureInfo, Task<TStringDictionary>> asyncLocalizedStringsRetriever,
            string resource,
            CultureInfo culture,
            TCultureDictionary localizedStrings)
        {
            this.localizedStringsRetriever = localizedStringsRetriever;
            this.asyncLocalizedStringsRetriever = asyncLocalizedStringsRetriever;
            this.resource = resource;
            this.culture = culture;
            this.LocalizedStrings = localizedStrings;
        }

        public NHStringLocalizer(Func<string, CultureInfo, TStringDictionary> localizedStringsRetriever,
            Func<string, CultureInfo, Task<TStringDictionary>> asyncLocalizedStringsRetriever,
            string resource)
        {
            this.localizedStringsRetriever = localizedStringsRetriever;
            this.asyncLocalizedStringsRetriever = asyncLocalizedStringsRetriever;
            this.resource = resource;
        }

        private TStringDictionary RetrieveLocalizedStrings()
        {
            var currentCulture = GetCurrentCulture();
            if (LocalizedStrings.TryGetValue(currentCulture, out var localizedStringsForCurrentCulture))
                return localizedStringsForCurrentCulture;
            localizedStringsForCurrentCulture = localizedStringsRetriever(resource, currentCulture);
            LocalizedStrings.Add(currentCulture, localizedStringsForCurrentCulture);
            return localizedStringsForCurrentCulture;
        }

        private async ValueTask<TStringDictionary> RetrieveLocalizedStringsAsync()
        {
            var currentCulture = GetCurrentCulture();
            if (LocalizedStrings.TryGetValue(currentCulture, out var localizedStringsForCurrentCulture))
            {
                return localizedStringsForCurrentCulture;
            }
            else
            {
                localizedStringsForCurrentCulture = await asyncLocalizedStringsRetriever(resource, currentCulture);
                LocalizedStrings.Add(currentCulture, localizedStringsForCurrentCulture);
                return localizedStringsForCurrentCulture;
            }
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

        IEnumerable<string> IReadOnlyDictionary<string, GetOrFormatLocalizedString>.Keys => LocalizedStrings[culture].Keys;

        IEnumerable<GetOrFormatLocalizedString> IReadOnlyDictionary<string, GetOrFormatLocalizedString>.Values => LocalizedStrings[culture].Values.Select(q => new GetOrFormatLocalizedString(q, Format));

        int IReadOnlyCollection<KeyValuePair<string, GetOrFormatLocalizedString>>.Count => LocalizedStrings[culture].Count;

        GetOrFormatLocalizedString IReadOnlyDictionary<string, GetOrFormatLocalizedString>.this[string key]
        {
            get
            {
                if (TryGetLocalizedString(key, out var value, out var localizedStringsForCurrentCulture))
                {
                    return value;
                }
                var notFoundResource = new LocalizedString(key, key, true, resource);
                localizedStringsForCurrentCulture.Add(key, notFoundResource);
                value = new GetOrFormatLocalizedString(notFoundResource, Format);
                return value;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new NHStringLocalizer<TCultureDictionary, TStringDictionary>(localizedStringsRetriever, asyncLocalizedStringsRetriever, resource, culture, LocalizedStrings);
        }

        public async ValueTask<IReadOnlyDictionary<string, GetOrFormatLocalizedString>> GetLocalizedStringsAsync()
        {
            await RetrieveLocalizedStringsAsync();
            return new NHStringLocalizer<TCultureDictionary, TStringDictionary>(localizedStringsRetriever, asyncLocalizedStringsRetriever, resource, GetCurrentCulture(), LocalizedStrings);
        }

        bool IReadOnlyDictionary<string, GetOrFormatLocalizedString>.ContainsKey(string key)
        {
            return LocalizedStrings[culture].ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, GetOrFormatLocalizedString>.TryGetValue(string key, out GetOrFormatLocalizedString value)
        {
            return TryGetLocalizedString(key, out value, out var localizedStringsForCurrentCulture);
        }

        private bool TryGetLocalizedString(string key, out GetOrFormatLocalizedString value, out TStringDictionary localizedStringsForCurrentCulture)
        {
            localizedStringsForCurrentCulture = LocalizedStrings[culture];
            if (localizedStringsForCurrentCulture.TryGetValue(key, out var localizedString))
            {
                value = new GetOrFormatLocalizedString(localizedString, Format);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private string Format(string value, object[] args)
        {
            return string.Format(value, args);
        }

        IEnumerator<KeyValuePair<string, GetOrFormatLocalizedString>> IEnumerable<KeyValuePair<string, GetOrFormatLocalizedString>>.GetEnumerator()
        {
            foreach(var localizedString in LocalizedStrings[culture])
            {
                yield return new KeyValuePair<string, GetOrFormatLocalizedString>(localizedString.Key, new GetOrFormatLocalizedString(localizedString.Value, Format));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var localizedString in LocalizedStrings[culture])
            {
                yield return new KeyValuePair<string, GetOrFormatLocalizedString>(localizedString.Key, new GetOrFormatLocalizedString(localizedString.Value, Format));
            }
        }
    }
}
