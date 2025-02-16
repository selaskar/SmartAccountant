using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using SmartAccountant.Abstractions.Interfaces;

namespace SmartAccountant.Identity;

internal sealed class AuthorizationService(IHttpContextAccessor httpContextAccessor) : IAuthorizationService
{
    /// <inheritdoc/>
    public Guid? UserId
    {
        get
        {
            IIdentity? identity = httpContextAccessor.HttpContext?.User.Identity;

            if (identity?.IsAuthenticated != true)
                return null;


            string? objectId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimConstants.ObjectId)?.Value;

            return objectId is null ? null : Guid.Parse(objectId);
        }
    }
}
