using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Models
{
    public class CultureModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string FlagFilename { get; set; }
    }

    public class CultureHtmlModel
    {
        public List<CultureModel> AvailableCultures { get; set; } = new List<CultureModel>();

        public string Username { get; set; }
    }
}
