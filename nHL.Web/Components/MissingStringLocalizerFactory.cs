using Microsoft.Extensions.Localization;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace nHL.Web.Components
{
    public class MissingStringLocalizerFactory : IStringLocalizerFactory, IAsyncLocalizerFactory
    {
        private readonly IStringLocalizerFactory inner;
        private readonly IAsyncLocalizerFactory asyncInner;
        private readonly IMissingStringLocalizerLogger missing;

        public MissingStringLocalizerFactory(IStringLocalizerFactory inner, IAsyncLocalizerFactory asyncInner, IMissingStringLocalizerLogger missing)
        {
            this.inner = inner;
            this.asyncInner = asyncInner;
            this.missing = missing;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new MissingStringLocalizer(inner.Create(resourceSource), missing);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new MissingStringLocalizer(inner.Create(baseName, location), missing);
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(Type resourceSource)
        {
            return new MissingStringLocalizer(asyncInner.Create(resourceSource), missing);
        }

        IAsyncStringLocalizer IAsyncLocalizerFactory.Create(string baseName, string location)
        {
            return new MissingStringLocalizer(asyncInner.Create(baseName, location), missing);
        }
    }
}
