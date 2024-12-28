using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace TicDrive.AppConfig
{
    public class UserClaimsMapper : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;

            var emailClaim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (emailClaim != null)
            {
                identity.AddClaim(new Claim("email", emailClaim.Value));
                identity.RemoveClaim(emailClaim);
            }

            var idClaim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (idClaim != null)
            {
                identity.AddClaim(new Claim("userId", idClaim.Value));
                identity.RemoveClaim(idClaim);
            }
        

            return Task.FromResult(principal);
        }
    }
}
