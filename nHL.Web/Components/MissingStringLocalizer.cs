using Microsoft.Extensions.Localization;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace nHL.Web.Components
{
    public class MissingStringLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer inner;
        private readonly IMissingStringLocalizerLogger missing;
        private readonly CultureInfo culture;

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

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new MissingStringLocalizer(inner.WithCulture(culture), missing, culture);
        }
    }
}
