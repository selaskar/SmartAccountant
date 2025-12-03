using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums;

public enum MainCategory : byte
{
    [Display(Name = nameof(ModelStrings.MainCategory_None), ResourceType = typeof(ModelStrings))]
    None = 0,
    [Display(Name = nameof(ModelStrings.MainCategory_Expense), ResourceType = typeof(ModelStrings))]
    Expense = 1,
    [Display(Name = nameof(ModelStrings.MainCategory_Income), ResourceType = typeof(ModelStrings))]
    Income = 2,
    [Display(Name = nameof(ModelStrings.MainCategory_Loan), ResourceType = typeof(ModelStrings))]
    Loan = 3,
    [Display(Name = nameof(ModelStrings.MainCategory_Saving), ResourceType = typeof(ModelStrings))]
    Saving = 4,
    [Display(Name = nameof(ModelStrings.MainCategory_InterestOrFee), ResourceType = typeof(ModelStrings))]
    InterestOrFee = 5,
}
