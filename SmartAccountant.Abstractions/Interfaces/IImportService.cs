using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IImportService
{
    /// <exception cref="ImportException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="OperationCanceledException" />
    /// <exception cref="ArgumentNullException"/>
    Task<Statement> ImportStatement(AbstractStatementImportModel request, CancellationToken cancellationToken);
}


public enum ImportableStatementTypes
{
    Debit,
    CreditCard,
}
