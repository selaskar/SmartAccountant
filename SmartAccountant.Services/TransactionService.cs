using System.Text;
using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Services;

internal class TransactionService(
    ITransactionRepository transactionRepository,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IValidator<DebitTransaction> debitTransactionValidator,
    IValidator<CreditCardTransaction> ccTransactionValidator)
    : ITransactionService
{
    private static readonly CompositeFormat AccountNotFound = CompositeFormat.Parse(Messages.AccountNotFound);

    /// <inheritdoc/>
    public async Task<Transaction[]> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        await VerifyAccountHolder(accountId, cancellationToken);

        try
        {
            return await transactionRepository.GetTransactionsOfAccount(accountId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new TransactionException(TransactionErrors.CannotFetchTransactionsOfAccount, ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateTransaction(DebitTransaction updateModel, CancellationToken cancellationToken)
    {
        debitTransactionValidator.ValidateAndThrowSafe(updateModel);

        await VerifyAccountHolder(updateModel.AccountId!.Value, cancellationToken);

        try
        {
            await transactionRepository.UpdateDebitTransaction(updateModel, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new TransactionException(TransactionErrors.CannotUpdateDebitTransaction, ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateTransaction(CreditCardTransaction updateModel, CancellationToken cancellationToken)
    {
        ccTransactionValidator.ValidateAndThrowSafe(updateModel);

        await VerifyAccountHolder(updateModel.AccountId!.Value, cancellationToken);

        try
        {
            await transactionRepository.UpdateCreditCardTransaction(updateModel, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new TransactionException(TransactionErrors.CannotUpdateCreditCardTransaction, ex);
        }
    }


    /// <exception cref="TransactionException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="AuthenticationException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task VerifyAccountHolder(Guid accountId, CancellationToken cancellationToken)
    {
        Account account = await accountRepository.GetAccount(accountId, cancellationToken)
                ?? throw new TransactionException(TransactionErrors.AccountNotFound, AccountNotFound.FormatMessage(accountId));

        if (account.HolderId != authorizationService.UserId)
            throw new TransactionException(TransactionErrors.AccountDoesNotBelongToUser);
    }
}
