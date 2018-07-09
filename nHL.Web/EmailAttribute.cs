using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace nHL.Web
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the EmailAttribute class.
        /// </summary>
        public EmailAttribute()
            : base(@"^([a-zA-Z0-9_äöüÄÖÜßăîâșțĂÎÂȘȚ\-\.]+)@((\[[0-9]{1,3}" +
                   @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9äöüÄÖÜßăîâșțĂÎÂȘȚ\-]+\" +
                   @".)+))([a-zA-ZäöüÄÖÜßăîâșțĂÎÂȘȚ]{2,4}|[0-9]{1,3})(\]?)$")
        {
        }
    }
}
