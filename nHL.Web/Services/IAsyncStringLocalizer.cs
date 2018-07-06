using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Services
{
    public interface IAsyncStringLocalizer
    {
        ValueTask<IReadOnlyDictionary<string, LocalizedString>> GetLocalizedStringsAsync();
    }

    public interface IAsyncStringLocalizer<T> : IAsyncStringLocalizer
    {

    }
}
