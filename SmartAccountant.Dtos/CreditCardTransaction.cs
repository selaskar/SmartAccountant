using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Dtos;

public class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}
