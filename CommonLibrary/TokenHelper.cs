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

        public async Task<string> GetAuthorizationToken(string tenantId, string resource, Uri uri, ClaimsPrincipal principal, string armToken)
        {
            AuthenticationContext context = new AuthenticationContext($"{_baseAuthority}{tenantId}");
            IPlatformParameters parameters = new PlatformParameters(PromptBehavior.Auto);

            string userObjectID = GetClaimValue(principal, Constantes.ClaimsType.ObjectIdentifier);

            UserAssertion assertion = new UserAssertion(armToken);
            ClientAssertion clientAssertion = new ClientAssertion(_clientId, armToken);
            ClientCredential credential = new ClientCredential(_clientId, _clientSecret);
            UserIdentifier identifier = new UserIdentifier(userObjectID, UserIdentifierType.UniqueId);

            AuthenticationResult result1 = null;
            string tk1 = null;
            try
            {
                result1 = await context.AcquireTokenAsync(resource, _clientId, uri, parameters, identifier);
                tk1 = result1.AccessToken;
            }
            catch
            {

            }
            AuthenticationResult result2 = null;
            try
            {
                result2 = await context.AcquireTokenAsync(resource, _clientId, uri, parameters);
            }
            catch
            {

            }
            AuthenticationResult result3 = null;
            try
            {
                result3 = await context.AcquireTokenAsync(resource, clientAssertion, assertion);
            }
            catch
            {

            }
            AuthenticationResult result4 = null;
            try
            {
                result4 = await context.AcquireTokenAsync(resource, clientAssertion);
            }
            catch
            {

            }
            AuthenticationResult result5 = null;
            try
            {
                 result5 = await context.AcquireTokenAsync(resource, credential, assertion); // on behalf on
            }
            catch
            {

            }
            AuthenticationResult result6 = null;
            try
            {
                result6 = await context.AcquireTokenAsync(resource, credential);
            }
            catch
            {

            }
            AuthenticationResult result7=null;
            try
            {
                result7 = await context.AcquireTokenAsync(resource, _clientId, uri, parameters, identifier);
            }
            catch
            {

            }




            //if (result == null)
            //{
            //    throw new InvalidOperationException(Resources.Jwt_Error);
            //}

            return result6.AccessToken;
        }
        #endregion
    }
}
