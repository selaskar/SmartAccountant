using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Models;

public record MonthlySummary : BaseModel
{
    public DateOnly Month { get; init; }

    public SummaryState State { get; set; }

    public IList<CurrencySummary> Currencies { get; init; } = [];
}
