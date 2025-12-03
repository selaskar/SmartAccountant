namespace SmartAccountant.Dtos;

public class CreditCard : AbstractCreditCard
{
    public IList<CreditCardLimit> Limits { get; private init; } = [];
}
