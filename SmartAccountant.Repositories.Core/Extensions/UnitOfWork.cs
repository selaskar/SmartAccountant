using Microsoft.EntityFrameworkCore.Storage;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;

namespace SmartAccountant.Repositories.Core.Extensions;

internal sealed class UnitOfWork(CoreDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (transaction is null)
            throw new InvalidOperationException("A transaction has not been started.");

        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (transaction is null)
            throw new InvalidOperationException("A transaction has not been started.");

        await transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        transaction?.Dispose();
    }
}
