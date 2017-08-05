#region using
using CommonLibrary.Model;
using CommonLibrary.Repositories;
using CommonLibrary.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks; 
#endregion

namespace CommonLibrary.Builders
{
    /// <summary>
    /// Builder to get the ViewModels needed to display Azure Resources Informations
    /// </summary>
    public sealed class AzureResourcesViewModelBuilder
    {
        #region Constructeur
        readonly ITokenHelper _tokenHelper;
        readonly IArmRepositories _armRepositories;

        public AzureResourcesViewModelBuilder(ITokenHelper tokenHelper, IArmRepositories armRepositories)
        {
            _tokenHelper = tokenHelper;
            _armRepositories = armRepositories;
        }
        #endregion

        #region GetTenants
        /// <summary>
        /// Get list of tenants the authenticated user is declared to
        /// </summary>
        /// <param name="principal">User's principal</param>
        /// <returns></returns>
        public async Task<AzureResourcesViewModel> GetTenants(ClaimsPrincipal principal, string tenantId = null)
        {
            AzureResourcesViewModel vm = new AzureResourcesViewModel();

            string currentTenantId = _tokenHelper.GetClaimValue(principal, Constantes.ClaimsType.TenantId);
            string armToken = await _tokenHelper.GetCurrentAuthorizationToken(currentTenantId);

            // all tenants
            string json = await _armRepositories.GetArmRequest("tenants", armToken);
            vm.Tenants = Tenant.Deserialize(json);

            if (tenantId != null)
            {
                currentTenantId = tenantId;

                armToken = await _tokenHelper.GetAuthorizationToken(currentTenantId, Constantes.Endpoints.ArmEndpoint);
                //http://www.cloudidentity.com/blog/2013/10/14/adal-windows-azure-ad-and-multi-resource-refresh-tokens/
            }

            // subscriptions
            json = await _armRepositories.GetArmRequest("subscriptions", armToken);
            vm.Subscriptions = Subscription.Deserialize(json);

            vm.Tenants.ForEach(t =>
            {
                t.IsCurrentTenant = t.TenantId == currentTenantId;
            });

            return vm;
        }
        #endregion
    }
}
