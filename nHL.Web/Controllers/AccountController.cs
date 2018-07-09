using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHibernate;
using NHibernate.Linq;
using nHL.Domain;
using nHL.Web.Filters;
using nHL.Web.Infrastructure.Persistence;
using nHL.Web.Models;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Controllers
{
    [TypeFilter(typeof(NHibernateSession<StatefulSessionWrapper>))]
    public class AccountController : Controller
    {
        private readonly ISession session;
        private readonly IAsyncStringLocalizer<AccountController> localizer;
        private readonly IAuthenticationService authenticationService;

        //these properties would be set by MVC if the controller was a POCO
        //public ControllerContext ActionContext { get; set; }
        //public ViewDataDictionary ViewData { get; set; }
        //public IUrlHelper Url { get; set; }

        public AccountController(ISession session, IAsyncStringLocalizer<AccountController> localizer, IAuthenticationService authenticationService)
        {
            this.session = session;
            this.localizer = localizer;
            this.authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult LogOn()
        {
            return View(new LogOnModel());
        }

        [HttpPost]
        public async Task<IActionResult> LogOn(LogOnModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await session.Query<User>().FirstOrDefaultAsync(item => item.Username.ToLower() == model.Username.ToLower() && item.Password == model.Password);
                if (user == null)
                {
                    var localizedMessages = await localizer.GetLocalizedStringsAsync();
                    ModelState.AddModelError("", localizedMessages["LogOnErrorMessage"]);
                    //
                    return View(model);
                }
                else
                {
                    //
                    // User logined succesfully ==> create a new site session!
                    //
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.UserRole.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var userPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await authenticationService.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties { IsPersistent = true });
                    //
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            await authenticationService.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { IsPersistent = true });
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterUserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if(ModelState.IsValid)
            {
                //
                // Verify if exists other user with the same username.
                //
                var existUser = await session.Query<User>().FirstOrDefaultAsync(item => item.Username.ToLower() == model.Username.ToLower());
                if (existUser == null)
                {
                    //
                    // Save the user data.
                    //
                    var user = new User
                    {
                        Username = model.Username,
                        Password = model.Password,
                        UserRole = UserRoles.SimpleUser,
                        Email = model.Email,
                        //
                        Address = new Address
                        {
                            Country = session.Load<Country>(model.Address.CountryId),
                            Fax = model.Address.Fax,
                            FirstName = model.Address.FirstName,
                            LastName = model.Address.LastName,
                            Phone = model.Address.Phone,
                            Street = model.Address.Street,
                            Town = model.Address.Town,
                            ZipCode = model.Address.ZipCode
                        }
                    };
                    //
                    await session.SaveAsync(user);
                    //
                    // Go to RegisterFinalized page!
                    //
                    return View("RegisterFinalized");
                }
                else
                {
                    //
                    // Exists other user with the same username, so show the error message.
                    //
                    var localizedMessages = await localizer.GetLocalizedStringsAsync();
                    ModelState.AddModelError("", localizedMessages["RegisterInvalidUsername"]);
                    //
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
