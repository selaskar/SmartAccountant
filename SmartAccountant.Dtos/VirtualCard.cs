namespace SmartAccountant.Dtos;

public class VirtualCard : AbstractCreditCard
{
    public Guid ParentId { get; init; }

    public Account? Parent { get; init; }
}
