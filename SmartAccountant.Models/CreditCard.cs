namespace SmartAccountant.Models;

public record class CreditCard : Account
{
    public CreditCard(IEnumerable<CreditCardLimit> historicLimits)
    {
        limits = [.. historicLimits];
    }

    private readonly SortedSet<CreditCardLimit> limits;

    public override BalanceType NormalBalance => BalanceType.Credit;

    public required string CardNumber { get; init; }

    public CreditCardLimit? GetLimit(DateTimeOffset asOf)
    {
        return limits.SingleOrDefault(l => l.Period.Overlaps(asOf));
    }
}
