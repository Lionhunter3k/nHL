using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace nHL.Domain
{
    public class Country
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Culture Culture { get; set; }
    }
}
