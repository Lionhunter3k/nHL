using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Activate Index page (the site Home page).
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Index()
        {
            // TO DO!
            return View();
        }

        /// <summary>
        /// Activate About page.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult About()
        {
            // TO DO!
            return View();
        }
    }
}
