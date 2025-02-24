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
    Task<Statement> ImportDebitStatement(DebitStatementImportModel request, CancellationToken cancellationToken);
    
    /*
    /// <exception cref="ImportException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="OperationCanceledException" />
    /// <exception cref="ArgumentNullException"/>
    Task<Statement> ImportCreditCardStatement(CreditCardStatementImportModel request, CancellationToken cancellationToken);*/
}
