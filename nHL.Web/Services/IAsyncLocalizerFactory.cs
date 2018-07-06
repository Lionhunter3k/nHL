using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Services
{
    public interface IAsyncLocalizerFactory
    {
        IAsyncStringLocalizer Create(Type resourceSource);
        IAsyncStringLocalizer Create(string baseName, string location);
    }
}
