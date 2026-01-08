using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;
using SmartAccountant.Repositories.Core.Resources;

namespace SmartAccountant.Repositories.Core;

internal sealed class TransactionRepository(CoreDbContext dbContext, IMapper mapper) : ITransactionRepository
{
    private static readonly CompositeFormat CannotFetchTransactionsOfUser = CompositeFormat.Parse(Messages.CannotFetchTransactionsOfUser);
    private static readonly CompositeFormat CannotFetchTransactionsOfUserForMonth = CompositeFormat.Parse(Messages.CannotFetchTransactionsOfUserForMonth);
    private static readonly CompositeFormat CannotUpdateDebitTransaction = CompositeFormat.Parse(Messages.CannotUpdateDebitTransaction);
    private static readonly CompositeFormat CannotUpdateCreditCardTransaction = CompositeFormat.Parse(Messages.CannotUpdateCreditCardTransaction);


    /// <inheritdoc/>
    public async Task<Models.Transaction[]> GetTransactionsOfAccount(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await dbContext.Transactions.AsNoTracking()
                .Include(x => x.Account)
                .Where(x => x.AccountId == accountId)
                .Select(x => mapper.Map<Models.Transaction>(x))
                .ToArrayAsync(cancellationToken);

            return transactions;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(CannotFetchTransactionsOfUser.FormatMessage(accountId), ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Models.Transaction[]> GetTransactionsOfMonth(Guid holderId, DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            Models.Transaction[] transactions = await dbContext.Transactions.AsNoTracking()
                .Include(t => t.Account)
                .Where(x => x.Account!.HolderId == holderId
                    && x.Timestamp.Year == month.Year
                    && x.Timestamp.Month == month.Month)
                .Select(x => mapper.Map<Models.Transaction>(x))
                .ToArrayAsync(cancellationToken);

            return transactions;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(CannotFetchTransactionsOfUserForMonth.FormatMessage(holderId, month), ex);
        }
    }

    /// <inheritdoc/>
    public async Task Insert(IEnumerable<Models.Transaction> transactions, CancellationToken cancellationToken)
    {
        try
        {
            var entities = mapper.Map<Entities.Transaction[]>(transactions);

            // Adds along statement documents.
            dbContext.Transactions.AddRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(Messages.CannotInsertTransactions, ex);
        }
    }

    /// <inheritdoc/>
    public async Task Delete(IEnumerable<Models.Transaction> transactions, CancellationToken cancellationToken)
    {
        try
        {
            var entities = mapper.Map<Entities.Transaction[]>(transactions);

            // Adds along statement documents.
            dbContext.Transactions.RemoveRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(Messages.CannotDeleteTransactions, ex);
        }
    }

    /// <inheritdoc/>
    public Task UpdateDebitTransaction(Models.DebitTransaction debitTransaction, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: electively update fields.
            var entity = mapper.Map<Entities.DebitTransaction>(debitTransaction);
            dbContext.Transactions.Update(entity);
            return dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(CannotUpdateDebitTransaction.FormatMessage(debitTransaction.Id), ex);
        }
    }

    /// <inheritdoc/>
    public Task UpdateCreditCardTransaction(Models.CreditCardTransaction creditCardTransaction, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: electively update fields.
            var entity = mapper.Map<Entities.CreditCardTransaction>(creditCardTransaction);
            dbContext.Transactions.Update(entity);
            return dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException(CannotUpdateCreditCardTransaction.FormatMessage(creditCardTransaction.Id), ex);
        }
    }
}
