using System.Security.Claims;
using System.Security.Principal;

namespace WebBanThoiTrang.Function
{
    public static class IdentityFunction
    {
        public static string GetAccountID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetRoleID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetAvatar(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetClaim(this ClaimsPrincipal principal, string claimType)
        {
            var claims = principal.Claims.FirstOrDefault(x => x.Type == claimType);
            return claims != null ? claims.Value : string.Empty;
        }
    }
}
