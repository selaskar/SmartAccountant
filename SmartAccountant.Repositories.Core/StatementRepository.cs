using AutoMapper;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;

namespace SmartAccountant.Repositories.Core;

internal sealed class StatementRepository(CoreDbContext dbContext, IMapper mapper) : IStatementRepository
{
    /// <inheritdoc/>
    public async Task Insert(Models.Statement statement, CancellationToken cancellationToken)
    {
        try
        {
            Entities.Statement entity = mapper.Map<Entities.Statement>(statement);

            // Adds along statement documents.
            dbContext.Statements.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ServerException($"Failed to insert statement ({statement.Id}).", ex);
        }
    }
}
