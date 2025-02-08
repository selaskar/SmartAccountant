using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;
using SmartAccountant.Repositories.Core.Entities;

namespace SmartAccountant.Repositories.Core;

internal sealed class AccountRepository(CoreDbContext dbContext, IMapper mapper) : IAccountRepository
{
    /// <inheritdoc/>
    public async Task<Models.Account?> GetAccount(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            Account? account = await dbContext.Accounts.AsNoTracking().
                SingleOrDefaultAsync(x => x.Id == accountId, cancellationToken);

            return mapper.Map<Models.Account?>(account);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to fetch account ({accountId}).", ex);
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<Models.Account> GetAccountsOfUser(Guid userId)
    {
        try
        {
            var accounts = dbContext.Accounts.AsNoTracking()
                .Where(x => x.HolderId == userId)
                .Select(x => mapper.Map<Models.Account>(x))
                .AsAsyncEnumerable();

            return accounts;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to fetch accounts of user ({userId}).", ex);
        }
    }
}
