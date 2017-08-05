#region using
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Linq;
using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using CommonLibrary.Resource;
#endregion

namespace CommonLibrary
{
    /// <summary>
    /// Methods helper to manipulate a token
    /// </summary>
    public sealed class TokenHelper : ITokenHelper
    {
        #region Constructor
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _baseAuthority;
        readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            _clientId = _configuration["Authentication:AzureAd:ClientId"];
            _clientSecret = _configuration["Authentication:AzureAd:Secret"];
            _baseAuthority = _configuration["Authentication:AzureAd:AADInstance"];
        }
        #endregion

        # region GetClaimValue
        /// <summary>
        /// Extract a value from a ClaimsPrincipal
        /// </summary>
        /// <param name="principal">ClaimsPrincipal to analyse</param>
        /// <param name="claimType">Claim's type</param>
        /// <returns>Value of the claim or null if not found</returns>
        public string GetClaimValue(ClaimsPrincipal principal, string claimType)
        {
            Claim claim = principal.Claims.Where(c => c.Type == claimType).FirstOrDefault();

            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }
        #endregion

        #region GetCurrentAuthorizationToken
        public async Task<string> GetCurrentAuthorizationToken(string tenantId)
        {
            AuthenticationContext context = new AuthenticationContext($"{_baseAuthority}{tenantId}");
            AuthenticationResult result = await context.AcquireTokenSilentAsync(Constantes.Endpoints.ArmEndpoint, _clientId);

            if (result == null)
            {
                throw new InvalidOperationException(Resources.Jwt_Error);
            }

            return result.AccessToken;
        }
        #endregion

        #region GetAuthorizationToken
        public async Task<string> GetAuthorizationToken(string tenantId, string resource)
        {
            AuthenticationContext context = new AuthenticationContext($"{_baseAuthority}{tenantId}");

            ClientCredential credential = new ClientCredential(_clientId, _clientSecret);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, credential);

            if (result == null)
            {
                throw new InvalidOperationException(Resources.Jwt_Error);
            }

            return result.AccessToken;
        }
        #endregion
    }
}
