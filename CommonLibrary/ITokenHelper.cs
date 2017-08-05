using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface ITokenHelper
    {
        string GetClaimValue(ClaimsPrincipal principal, string claimType);
        Task<string> GetCurrentAuthorizationToken(string tenantId);
        Task<string> GetAuthorizationToken(string tenantId, string resource);
    }
}