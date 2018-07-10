using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using nHL.Domain;
using nHL.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.ViewComponents
{
    public class LanguageMenuViewComponent : ViewComponent
    {
        private readonly ISession session;

        public LanguageMenuViewComponent(ISession session)
        {
            this.session = session;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cultures = await session.Query<Culture>().Where(q => q.Disabled == false).ToListAsync();
            var model = new CultureHtmlModel
            {
                AvailableCultures = cultures.Select(q => new CultureModel { DisplayName = q.DisplayName, FlagFilename = q.FlagFilename, Name = q.Name }).ToList()
            };
            var rawId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(rawId, out int id))
            {
                model.Username = (await session.GetAsync<User>(id)).Username;
            }
            return View(model);
        }
    }
}
