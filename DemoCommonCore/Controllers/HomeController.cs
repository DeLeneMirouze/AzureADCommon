using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DemoCommonCore.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        #region Constructor
        public HomeController(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "AzureResources");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
