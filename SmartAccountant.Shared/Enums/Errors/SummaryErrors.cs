using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Abstractions.Exceptions;

public enum SummaryErrors
{
    [Display(Name = nameof(ModelStrings.SummaryErrors_Unspecified), ResourceType = typeof(ModelStrings))]
    Unspecified = 0,
    [Display(Name = nameof(ModelStrings.SummaryErrors_CannotCalculateSummary), ResourceType = typeof(ModelStrings))]
    CannotCalculateSummary = 1,
}
