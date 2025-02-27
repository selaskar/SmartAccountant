namespace SmartAccountant.Models;

//TODO: probably need to implement IEquatable for the Set, too.
public record class CreditCardLimit : BaseModel, IComparable<CreditCardLimit>
{
    public Guid CreditCardId { get; init; }

    public decimal Amount { get; init; }

    public Period Period { get; init; }

    /// <exception cref="ArgumentNullException"/>
    public int CompareTo(CreditCardLimit? other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Period.CompareTo(other.Period);
    }
    public static bool operator <(CreditCardLimit left, CreditCardLimit right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(CreditCardLimit left, CreditCardLimit right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(CreditCardLimit left, CreditCardLimit right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(CreditCardLimit left, CreditCardLimit right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}
