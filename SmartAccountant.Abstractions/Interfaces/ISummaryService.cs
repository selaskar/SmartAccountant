using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISummaryService
{
    /// <exception cref="SummaryException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<MonthlySummary> GetSummary(DateOnly month, CancellationToken cancellationToken);
}
