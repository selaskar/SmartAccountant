using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Abstract;

public interface IAccountRepository
{
    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException"/>
    Task<Account?> GetAccount(Guid accountId, CancellationToken cancellationToken);

    /// <exception cref="RepositoryException" />
    IAsyncEnumerable<Account> GetAccountsOfUser(Guid userId);
    
    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException"/>
    Task<IEnumerable<Balance>> GetBalancesOfUser(Guid userId, DateTimeOffset asOf, CancellationToken cancellationToken);

    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException"/>
    Task<IEnumerable<CreditCardLimit>> GetLimitsOfUser(Guid userId, DateTimeOffset asOf, CancellationToken cancellationToken);

    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException"/>
    Task SaveBalance(Balance balance, CancellationToken cancellationToken);
}
