namespace SmartAccountant.Repositories.Core.Abstract;

public interface IUnitOfWork : IDisposable
{
    /// <exception cref="OperationCanceledException"/>
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    /// <exception cref="OperationCanceledException"/>
    Task CommitAsync(CancellationToken cancellationToken);

    /// <exception cref="OperationCanceledException"/>
    Task RollbackAsync(CancellationToken cancellationToken);
}
