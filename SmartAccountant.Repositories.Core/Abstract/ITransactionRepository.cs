using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Abstract;

public interface ITransactionRepository
{
    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException" />
    Task<Transaction[]> GetTransactionsOfAccount(Guid accountId, CancellationToken cancellationToken);

    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException" />
    Task Insert(IEnumerable<Transaction> transactions, CancellationToken cancellationToken);

    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException" />
    Task Delete(IEnumerable<Transaction> transactions, CancellationToken cancellationToken);
}
