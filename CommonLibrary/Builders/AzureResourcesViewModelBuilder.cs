#region using
using CommonLibrary.Model;
using CommonLibrary.Repositories;
using CommonLibrary.ViewModels;
using Newtonsoft.Json.Linq;
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
        readonly IAzureRepositories _azureRepositories;

        public AzureResourcesViewModelBuilder(ITokenHelper tokenHelper, IAzureRepositories azureRepositories)
        {
            _tokenHelper = tokenHelper;
            _azureRepositories = azureRepositories;
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
            string json = await _azureRepositories.GetArmRequest("tenants", armToken);
            vm.Tenants = Tenant.DeserializeTenantList(json);

            //foreach (Tenant tenant in vm.Tenants)
            //{
            //    Uri uri = new Uri("https://localhost:44361/signin-oidc");
            //    string graphToken = await _tokenHelper.GetAuthorizationToken(tenant.TenantId, Constantes.Endpoints.GraphEndpoint, uri, principal, armToken);
            //    json = await _azureRepositories.GetGraphRequest($"{tenant.TenantId}/tenantDetails", graphToken);

            //    Tenant.DeserializeTenantInfo(json, tenant);
            //}

            //if (tenantId != null)
            //{
            //    currentTenantId = tenantId;

            //    //armToken = await _tokenHelper.GetAuthorizationToken(currentTenantId, Constantes.Endpoints.ArmEndpoint);
            //    //armToken = await _tokenHelper.GetAuthorizationToken(currentTenantId, $"https://graph.windows.net/{currentTenantId}/tenantDetails");
            //    armToken = await _tokenHelper.GetAuthorizationToken(currentTenantId, Constantes.Endpoints.GraphEndpoint);
            //    json = await _azureRepositories.GetGraphRequest($"{currentTenantId}/tenantDetails", armToken);
            //    //http://www.cloudidentity.com/blog/2013/10/14/adal-windows-azure-ad-and-multi-resource-refresh-tokens/
            //    //https://graph.windows.net/{currentTenantId}/tenantDetails
            //}

            // subscriptions
            json = await _azureRepositories.GetArmRequest("subscriptions", armToken);
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
