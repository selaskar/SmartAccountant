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
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RepositoryException($"Failed to fetch account ({accountId}).", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Models.Account[]> GetAccountsOfUser(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = await dbContext.Accounts.AsNoTracking()
                .Where(x => x.HolderId == userId)
                .Select(x => mapper.Map<Models.Account>(x))
                .ToArrayAsync(cancellationToken);

            return accounts;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RepositoryException($"Failed to fetch accounts of user ({userId}).", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Models.CreditCardLimit>> GetLimitsOfUser(Guid userId, DateTimeOffset asOf, CancellationToken cancellationToken)
    {
        try
        {
            List<CreditCardLimit> limits = await dbContext.CreditCardLimits.AsNoTracking()
                .Where(l => l.Card!.HolderId == userId && l.ValidSince <= asOf && l.ValidUntil >= asOf)
                .GroupBy(l => l.Id, (cardId, limits) => limits.OrderByDescending(l => l.ValidSince).First())
                .ToListAsync(cancellationToken);

            return limits.Select(mapper.Map<Models.CreditCardLimit>);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RepositoryException($"Failed to fetch credit card limits for user ({userId}).", ex);
        }
    }
}
