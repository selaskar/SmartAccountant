using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using SmartAccountant.Abstractions.Interfaces;

namespace SmartAccountant.Identity;

internal sealed class AuthorizationService(IHttpContextAccessor httpContextAccessor) : IAuthorizationService
{
    /// <inheritdoc/>
    public Guid? UserId
    {
        get
        {
            IIdentity? identity = httpContextAccessor.HttpContext.User.Identity;

            if (identity?.IsAuthenticated != true)
                return null;

            string? sid = httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

            return sid is null ? null : Guid.Parse(sid);
        }
    }
}
