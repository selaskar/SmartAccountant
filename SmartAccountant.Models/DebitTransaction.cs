namespace SmartAccountant.Models;

public record class DebitTransaction : Transaction
{
    /// <summary>
    /// Balance after the transaction
    /// </summary>
    public MonetaryValue RemainingBalance { get; init; }

    /// <summary>
    /// Zero-based order of the transaction in the statement
    /// </summary>
    public short Order { get; init; }
}
