namespace SmartAccountant.Models;

public class DebitStatement : Statement<DebitTransaction>
{
    public Currency Currency { get; init; }
}
