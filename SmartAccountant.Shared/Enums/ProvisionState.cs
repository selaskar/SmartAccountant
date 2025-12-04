using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums;

public enum ProvisionState : byte
{
    [Display(Name = nameof(ModelStrings.ProvisionState_Finalized), ResourceType = typeof(ModelStrings))]
    Finalized = 0,
    [Display(Name = nameof(ModelStrings.ProvisionState_Open), ResourceType = typeof(ModelStrings))]
    Open = 1
}
