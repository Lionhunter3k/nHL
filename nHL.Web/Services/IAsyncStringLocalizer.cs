using Microsoft.Extensions.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Services
{
    public interface IAsyncStringLocalizer
    {
        ValueTask<IReadOnlyDictionary<string, GetOrFormatLocalizedString>> GetLocalizedStringsAsync();
    }

    public interface IAsyncStringLocalizer<T> : IAsyncStringLocalizer
    {

    }
}
