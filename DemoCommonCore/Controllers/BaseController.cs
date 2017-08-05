#region using
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Globalization; 
#endregion

namespace DemoCommonCore.Controllers
{
    public class BaseController : Controller
    {
        #region Constructeur
        public BaseController(IConfiguration configuration)
        {
            Configuration = configuration;

            string culture = Configuration["app:culture"];
            if (!string.IsNullOrWhiteSpace(culture))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            }
        }
        #endregion

        protected IConfiguration Configuration { get; set; }

        #region CallbackUri
        /// <summary>
        /// Redirect Uri
        /// </summary>
        /// <returns></returns>
        public Uri CallbackUri()
        {
            string callbackPath = Configuration["Authentication:AzureAd:CallbackPath"];
            string callbackUrl = Request.Scheme + "://" + Request.Host + callbackPath;
            return new Uri(callbackUrl);
        } 
        #endregion
    }
}