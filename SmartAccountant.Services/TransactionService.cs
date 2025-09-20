using System.Text;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services;

internal class TransactionService(ITransactionRepository transactionRepository, IAuthorizationService authorizationService, IAccountRepository accountRepository)
    : ITransactionService
{
    private static readonly CompositeFormat AccountNotFound = CompositeFormat.Parse(Messages.AccountNotFound);

    /// <inheritdoc/>
    public async Task<Transaction[]> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            Account account = await accountRepository.GetAccount(accountId, cancellationToken)
                ?? throw new TransactionException(AccountNotFound.FormatMessage(accountId), null);

            if (account.HolderId != authorizationService.UserId)
                throw new TransactionException(Messages.AccountDoesNotBelongToUser, null);

            return await transactionRepository.GetTransactionsOfAccount(accountId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not TransactionException)
        {
            throw new TransactionException(Messages.CannotFetchTransactionsOfAccount, ex);
        }
    }
}
