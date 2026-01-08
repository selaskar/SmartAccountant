using System.Text;
using AutoMapper;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;
using SmartAccountant.Repositories.Core.Resources;

namespace SmartAccountant.Repositories.Core;

internal sealed class StatementRepository(CoreDbContext dbContext, IMapper mapper) : IStatementRepository
{
    private static readonly CompositeFormat CannotInsertStatement = CompositeFormat.Parse(Messages.CannotInsertStatement);

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
            throw new ServerException(CannotInsertStatement.FormatMessage(statement.Id), ex);
        }
    }
}
