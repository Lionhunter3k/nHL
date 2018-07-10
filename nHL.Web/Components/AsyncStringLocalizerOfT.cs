using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    public class AsyncStringLocalizer<T> : IAsyncStringLocalizer<T>
    {
        private readonly IAsyncStringLocalizer localizer;

        public AsyncStringLocalizer(IAsyncLocalizerFactory asyncLocalizerFactory)
        {
            this.localizer = asyncLocalizerFactory.Create(typeof(T));
        }

        public ValueTask<IReadOnlyDictionary<string, GetOrFormatLocalizedString>> GetLocalizedStringsAsync()
        {
            return localizer.GetLocalizedStringsAsync();
        }
    }
}
