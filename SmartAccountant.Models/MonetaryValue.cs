using System.Diagnostics.CodeAnalysis;

namespace SmartAccountant.Models;

public readonly record struct MonetaryValue(decimal Amount, Currency Currency)
{
    public static MonetaryValue operator *(MonetaryValue left, decimal right)
        => new(left.Amount * right, left.Currency);

    public static MonetaryValue Multiply(MonetaryValue left, decimal right) => left * right;
}

[SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "We map this enum to a database column.")]
public enum Currency : short
{
    USD = 0,
    EUR = 1,
    TRY = 2,
}
