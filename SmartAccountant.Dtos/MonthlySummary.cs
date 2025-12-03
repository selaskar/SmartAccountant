namespace SmartAccountant.Dtos;

public class MonthlySummary : BaseDto
{
    public DateOnly Month { get; init; }

    public IList<CurrencySummary> Currencies { get; init; } = [];
}
