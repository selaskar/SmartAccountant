namespace SmartAccountant.Models;

public record MonthlySummary : BaseModel
{
    public DateOnly Month { get; init; }

    public IList<CurrencySummary> Currencies { get; init; } = [];
}
