using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using GamingPlatform6.Models;
using GamingPlatform6.Models.Home;
using System.Diagnostics;
using GamingPlatform6;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Xml.Linq;

//HomeController.cs================================================================
namespace GamingPlatform6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IHtmlLocalizer<SharedResource> _htmlLocalizer;

        /* Here is of course the Dependency Injection (DI) coming in and filling 
         * all the dependencies. The key thing is we are asking for a specific 
         * type=SharedResource. 
         * If it doesn't work for you, you can try to use full class name
         * in your DI instruction, like this one:
         * IStringLocalizer<GamingPlatform6.SharedResource> stringLocalizer
         */
        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> stringLocalizer,
            IHtmlLocalizer<SharedResource> htmlLocalizer)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _htmlLocalizer = htmlLocalizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ChangeLanguage(ChangeLanguageViewModel model)
        {
            if (model.IsSubmit)
            {
                HttpContext myContext = this.HttpContext;
                ChangeLanguage_SetCookie(myContext, model.SelectedLanguage);
                //doing funny redirect to get new Request Cookie
                //for presentation
                return LocalRedirect("/Home/ChangeLanguage");
            }

            //prepare presentation
            ChangeLanguage_PreparePresentation(model);
            return View(model);
        }

        private void ChangeLanguage_PreparePresentation(ChangeLanguageViewModel model)
        {
            model.ListOfLanguages = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Text = "English",
                                Value = "en"
                            },

                            new SelectListItem
                            {
                                Text = "French",
                                Value = "fr"
                            }
                        };
        }

        private void ChangeLanguage_SetCookie(HttpContext myContext, string? culture)
        {
            if (culture == null) { throw new Exception("culture == null"); };

            //this code sets .AspNetCore.Culture cookie
            myContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
            );
        }

        public IActionResult LocalizationExample(LocalizationExampleViewModel model)
        {
            //so, here we use IStringLocalizer
            model.IStringLocalizerInController = _stringLocalizer["Wellcome"];
            model.IStringLocalizerInController = _stringLocalizer["WelcomeMessage"];
            //so, here we use IHtmlLocalizer
            model.IHtmlLocalizerInController = _htmlLocalizer["Wellcome"];
            model.IHtmlLocalizerInController = _htmlLocalizer["WelcomeMessage"];
            return View(model);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
