using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISummaryService
{
    /// <exception cref="SummaryException"/>
    Task<MonthlySummary> GetSummary(DateOnly month, CancellationToken cancellationToken);
}
