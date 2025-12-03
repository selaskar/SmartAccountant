using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Dtos;

public class DebitTransaction : Transaction
{
    /// <summary>
    /// Balance after the transaction
    /// </summary>
    public MonetaryValue RemainingBalance { get; set; }
}
