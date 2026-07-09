using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public record class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}

public record class XCreditCardTransaction : CreditCardTransaction //TODO: a meaningful multipart, shared, 
{

}
