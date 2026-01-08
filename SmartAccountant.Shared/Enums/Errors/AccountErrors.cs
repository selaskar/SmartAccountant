using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums.Errors;

public enum AccountErrors
{
    [Display(Name = nameof(ModelStrings.AccountErrors_CannotFetchAccountsOfUser), ResourceType = typeof(ModelStrings))]
    CannotFetchAccountsOfUser = 1,
}
