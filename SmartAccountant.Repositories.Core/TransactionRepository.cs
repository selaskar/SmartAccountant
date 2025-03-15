using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;

namespace SmartAccountant.Repositories.Core;

internal sealed class TransactionRepository(CoreDbContext dbContext, IMapper mapper) : ITransactionRepository
{
    /// <inheritdoc/>
    public async Task<Models.Transaction[]> GetTransactionsOfAccount(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: take period as filter parameter.
            var accounts = await dbContext.Transactions.AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .Select(x => mapper.Map<Models.Transaction>(x))
                .ToArrayAsync(cancellationToken);

            return accounts;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to fetch transactions of account ({accountId}).", ex);
        }
    }

    /// <inheritdoc/>
    public async Task Insert(IEnumerable<Models.Transaction> transactions, CancellationToken cancellationToken)
    {
        try
        {
            Entities.Transaction[] entities = mapper.Map<Entities.Transaction[]>(transactions);

            // Adds along statement documents.
            dbContext.Transactions.AddRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to insert transactions.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task Delete(IEnumerable<Models.Transaction> transactions, CancellationToken cancellationToken)
    {
        try
        {
            Entities.Transaction[] entities = mapper.Map<Entities.Transaction[]>(transactions);

            // Adds along statement documents.
            dbContext.Transactions.RemoveRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to remove transactions.", ex);
        }
    }
}


