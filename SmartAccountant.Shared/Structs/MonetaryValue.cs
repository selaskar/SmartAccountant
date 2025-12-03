using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Shared.Structs;

public readonly record struct MonetaryValue(decimal Amount, Currency Currency)
{
    public static MonetaryValue operator +(MonetaryValue left, MonetaryValue right)
        => new(left.Amount + right.Amount, left.Currency);

    public static MonetaryValue operator -(MonetaryValue left, MonetaryValue right)
        => new(left.Amount - right.Amount, left.Currency);

    public static MonetaryValue operator *(MonetaryValue left, decimal right)
        => new(left.Amount * right, left.Currency);

    public static MonetaryValue Add(MonetaryValue left, MonetaryValue right) => left + right;

    public static MonetaryValue Subtract(MonetaryValue left, MonetaryValue right) => left - right;

    public static MonetaryValue Multiply(MonetaryValue left, decimal right) => left * right;
}
