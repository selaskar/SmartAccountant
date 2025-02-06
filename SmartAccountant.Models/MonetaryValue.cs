namespace SmartAccountant.Models;

public readonly record struct MonetaryValue(decimal Amount, Currency Currency)
{
}

public enum Currency
{
    USD = 0,
    EUR = 1,
    TRY = 2,
}
