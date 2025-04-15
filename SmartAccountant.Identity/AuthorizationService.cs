using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Identity.Resources;

namespace SmartAccountant.Identity;

internal sealed class AuthorizationService(IHttpContextAccessor httpContextAccessor) : IAuthorizationService
{
    /// <inheritdoc/>
    public Guid UserId
    {
        get
        {
            IIdentity? identity = httpContextAccessor.HttpContext?.User.Identity;

            if (identity?.IsAuthenticated != true)
                throw new AuthenticationException(Messages.UserNotAuthenticated);


            string? objectId = httpContextAccessor.HttpContext!.User.FindFirst("oid")?.Value;

            return objectId is not null ? Guid.Parse(objectId)
                : throw new AuthenticationException(Messages.UserNotAuthenticated);
        }
    }
}
