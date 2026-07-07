
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Abstract;

public interface ISummaryRepository
{
    Task<MonthlySummary?> GetSummary(Guid userId, DateOnly month, CancellationToken cancellationToken);
    Task UpdateSummary(MonthlySummary monthlySummary, CancellationToken cancellationToken);
}
