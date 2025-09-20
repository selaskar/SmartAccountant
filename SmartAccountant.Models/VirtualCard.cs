namespace SmartAccountant.Models;

public record class VirtualCard : AbstractCreditCard
{
    public Guid ParentId { get; init; }

    public Account? Parent { get; init; }
}
