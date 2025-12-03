using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Dtos;

public class CreditCardLimit : BaseDto
{
    public Guid CreditCardId { get; init; }

    public MonetaryValue Amount { get; init; }

    public Period Period { get; init; }
}
