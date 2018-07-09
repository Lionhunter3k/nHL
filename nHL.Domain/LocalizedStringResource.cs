using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Domain
{
    public class LocalizedStringResource
    {
        public virtual string Resource { get; set; }

        public virtual string Key { get; set; }

        public virtual string Text { get; set; }

        public virtual Culture Culture { get; set; }

        public virtual bool ResourceNotFound { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is LocalizedStringResource localizedStringResource)
            {
                return localizedStringResource?.Key == this.Key && localizedStringResource?.Resource == this.Resource;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new ValueTuple<string,string>(this.Key, this.Resource).GetHashCode();
        }
    }
}
