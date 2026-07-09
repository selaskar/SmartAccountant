using AutoMapper;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;

namespace SmartAccountant.Repositories.Core;

internal class SummaryRepository(CoreDbContext dbContext, IMapper mapper) : ISummaryRepository
{
    public Task<MonthlySummary?> GetSummary(Guid userId, DateOnly month, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateSummary(MonthlySummary monthlySummary, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
