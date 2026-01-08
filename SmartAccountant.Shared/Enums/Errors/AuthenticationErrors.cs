using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum AuthenticationErrors
{
    [Display(Name = nameof(ModelStrings.AuthenticationErrors_UserNotAuthenticated), ResourceType = typeof(ModelStrings))]
    UserNotAuthenticated = 1,
}
