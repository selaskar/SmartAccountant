using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Repositories.Core.Abstract;

public interface IStatementRepository
{
    /// <exception cref="RepositoryException" />
    /// <exception cref="OperationCanceledException" />
    Task Insert(Statement statement, CancellationToken cancellationToken);
}
