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

    //TODO: delete
    /// <inheritdoc/>
    public async Task<IEnumerable<Models.Balance>> GetBalancesOfUser(Guid userId, DateTimeOffset asOf, CancellationToken cancellationToken)
    {
        try
        {
            List<Balance> balances = await dbContext.Balances.AsNoTracking()
                .Where(b => b.SavingAccount!.HolderId == userId && b.AsOf <= asOf)
                .GroupBy(b => b.SavingAccountId, (key, grouping) => grouping.OrderByDescending(g => g.AsOf).First())
                .ToListAsync(cancellationToken);

            Guid[] accountIds = balances.Select(b => b.SavingAccountId).ToArray();

            List<SavingAccount> accounts = await dbContext.SavingAccounts.AsNoTracking()
                .Where(a => accountIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            balances.ForEach(b => b.SavingAccount = accounts.Single(a => a.Id == b.SavingAccountId));

            return balances.Select(mapper.Map<Models.Balance>);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RepositoryException($"Failed to fetch account balances of user ({userId}).", ex);
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

    /// <inheritdoc/>
    public async Task SaveBalance(Models.Balance balance, CancellationToken cancellationToken)
    {
        try
        {
            var entity = mapper.Map<Balance>(balance);

            dbContext.Balances.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RepositoryException("Failed to save balance.", ex);
        }
    }
}
