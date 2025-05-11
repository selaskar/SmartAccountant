namespace SmartAccountant.Models;

public record class CreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    public required string CardNumber { get; init; }

    public IList<CreditCardLimit> Limits { get; private init; } = [];

    public CreditCardLimit? GetLimit(DateTimeOffset asOf)
    {
        return Limits.SingleOrDefault(l => l.Period.Overlaps(asOf));
    }
}
