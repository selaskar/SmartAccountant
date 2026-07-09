using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public record class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}
