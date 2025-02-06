namespace SmartAccountant.Models;

public class DebitTransaction : Transaction
{
    /// <summary>
    /// Balance after the transaction
    /// </summary>
    public MonetaryValue RemainingBalance { get; init; }
}
