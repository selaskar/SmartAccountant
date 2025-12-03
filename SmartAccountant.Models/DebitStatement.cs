using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public record class DebitStatement : Statement<DebitTransaction>
{
    public Currency Currency { get; init; }

    public decimal RemainingBalance { get; set; }
}
