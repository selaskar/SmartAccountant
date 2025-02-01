namespace SmartAccountant.Models;

public readonly record struct MonetaryValue
{
    public decimal Amount { get; init; }

    public Currency Currency { get; init; }
}

public enum Currency
{
    USD = 0,
    EUR = 1,
    TRY = 2,
}
