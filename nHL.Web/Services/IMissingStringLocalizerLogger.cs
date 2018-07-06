using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Services
{
    public interface IMissingStringLocalizerLogger
    {
        Task FlushMissingStringsAsync();

        void LogMissingLocalization(string location, string name, CultureInfo culture);
    }
}
