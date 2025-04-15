using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IImportService
{
    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="AuthenticationException"/>
    Task<Statement> ImportStatement(AbstractStatementImportModel request, CancellationToken cancellationToken);
}


public enum ImportableStatementTypes
{
    Debit,
    CreditCard,
}
