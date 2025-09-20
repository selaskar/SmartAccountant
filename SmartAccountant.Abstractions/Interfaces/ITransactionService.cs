using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ITransactionService
{
    /// <exception cref="TransactionException"/>
    /// <exception cref="AuthenticationException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<Transaction[]> GetTransactions(Guid accountId, CancellationToken cancellationToken);
}
