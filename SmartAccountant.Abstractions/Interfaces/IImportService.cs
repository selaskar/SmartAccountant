using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IImportService
{
    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ValidationException"/>
    Task<Statement> ImportStatement(AbstractStatementImportModel model, CancellationToken cancellationToken);
}


public enum ImportableStatementTypes
{
    Debit,
    CreditCard,
    Multipart,
}
