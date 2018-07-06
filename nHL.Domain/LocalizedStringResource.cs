using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Domain
{
    public class LocalizedStringResource
    {
        public virtual string Key { get; set; }

        public virtual string Resource { get; set; }

        public virtual string Text { get; set; }

        public virtual Culture Culture { get; set; }

    }
}
