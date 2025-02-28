namespace SmartAccountant.Models;

public record struct Period : IComparable<Period>
{
    public DateTimeOffset ValidFrom { get; init; }

    public DateTimeOffset? ValidTo { get; set; }

    /// <remarks>Inclusive</remarks>
    public readonly bool Overlaps(DateTimeOffset date) => ValidFrom <= date && (ValidTo == null || ValidTo >= date);

    /// <remarks>Exclusive</remarks>
    public readonly bool Overlaps(Period other) => (other.ValidTo == null || other.ValidTo > ValidFrom) && (ValidTo == null || other.ValidFrom < ValidTo);

    public readonly int CompareTo(Period other)
    {
        if (Overlaps(other))
            return 0;

        return ValidFrom < other.ValidFrom ? -1 : 1;
    }

    public static bool operator <(Period left, Period right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Period left, Period right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Period left, Period right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Period left, Period right)
    {
        return left.CompareTo(right) >= 0;
    }
}
