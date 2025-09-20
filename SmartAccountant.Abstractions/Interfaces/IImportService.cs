using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IImportService
{
    /// <exception cref="ImportException"/>
    /// <exception cref="AuthenticationException"/>
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
