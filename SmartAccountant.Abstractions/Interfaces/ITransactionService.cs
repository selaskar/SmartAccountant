using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ITransactionService
{
    /// <exception cref="TransactionException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<Transaction[]> GetTransactions(Guid accountId, CancellationToken cancellationToken);

    /// <exception cref="TransactionException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="ArgumentNullException"/>
    Task UpdateTransaction(DebitTransaction updateModel, CancellationToken cancellationToken);

    /// <exception cref="TransactionException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="ArgumentNullException"/>
    Task UpdateTransaction(CreditCardTransaction updateModel, CancellationToken cancellationToken);
}
