#region using
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using CommonLibrary;
using CommonLibrary.Repositories;
using CommonLibrary.ViewModels;
using CommonLibrary.Builders;
using Microsoft.Extensions.Configuration;
using System;
#endregion

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoCommonCore.Controllers
{
    public class AzureResourcesController : BaseController
    {
        #region Constructor
        readonly AzureResourcesViewModelBuilder _azureResourcesViewModelBuilder;
        readonly IArmRepositories _armRepositories;

        public AzureResourcesController(ITokenHelper tokenHelper, IArmRepositories armRepositories, IConfiguration configuration):base(configuration)
        {
            _armRepositories = armRepositories;
            _azureResourcesViewModelBuilder = new AzureResourcesViewModelBuilder(tokenHelper, armRepositories);
        }
        #endregion

        #region Index
        // GET: /<controller>/
        public async Task<IActionResult> Index(string tenantId = null)
        {
            try
            {
                AzureResourcesViewModel vm = await _azureResourcesViewModelBuilder.GetTenants(User, tenantId);

                return View(vm);
            }
            catch (AdalSilentTokenAcquisitionException)
            {
                return RedirectToAction("SignOut", "Account");
            }
        } 
        #endregion
    }
}
