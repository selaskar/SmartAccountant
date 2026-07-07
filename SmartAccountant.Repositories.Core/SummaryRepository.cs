using AutoMapper;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;

namespace SmartAccountant.Repositories.Core;

internal class SummaryRepository(CoreDbContext dbContext, IMapper mapper) : ISummaryRepository
{
}
