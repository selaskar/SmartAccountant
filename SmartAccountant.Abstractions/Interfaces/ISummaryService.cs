using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISummaryService
{
    //TODO:
    Task<MonthlySummary> GetSummary(DateOnly month, CancellationToken cancellationToken);
}
