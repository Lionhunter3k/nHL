﻿using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Domain
{
    public class Culture
    {
        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual bool Disabled { get; set; }

        public virtual string FlagFilename { get; set; }
    }
}
