using System.Text;
using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services;

internal class TransactionService(
    ITransactionRepository transactionRepository, 
    IAuthorizationService authorizationService, 
    IAccountRepository accountRepository,
    IValidator<DebitTransaction> validator)
    : ITransactionService
{
    private static readonly CompositeFormat AccountNotFound = CompositeFormat.Parse(Messages.AccountNotFound);

    /// <inheritdoc/>
    public async Task<Transaction[]> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            await VerifyAccountHolder(accountId, cancellationToken);

            return await transactionRepository.GetTransactionsOfAccount(accountId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not TransactionException)
        {
            throw new TransactionException(Messages.CannotFetchTransactionsOfAccount, ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateTransaction(DebitTransaction updateModel, CancellationToken cancellationToken)
    {
        try
        {
            validator.ValidateAndThrowSafe(updateModel);

            await VerifyAccountHolder(updateModel.AccountId!.Value, cancellationToken);

            await transactionRepository.UpdateDebitTransaction(updateModel, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not TransactionException)
        {
            throw new TransactionException(Messages.CannotUpdateDebitTransaction, ex);
        }
    }


    /// <exception cref="TransactionException"/>
    private async Task VerifyAccountHolder(Guid accountId, CancellationToken cancellationToken)
    {
        Account account = await accountRepository.GetAccount(accountId, cancellationToken)
                ?? throw new TransactionException(AccountNotFound.FormatMessage(accountId), null);

        if (account.HolderId != authorizationService.UserId)
            throw new TransactionException(Messages.AccountDoesNotBelongToUser, null);
    }
}
