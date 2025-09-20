namespace SmartAccountant.Models;

public record class CreditCardLimit : BaseModel
{
    public Guid CreditCardId { get; init; }

    public MonetaryValue Amount { get; init; }

    public Period Period { get; init; }
}
