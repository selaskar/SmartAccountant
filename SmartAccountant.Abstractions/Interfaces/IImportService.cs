using FluentValidation;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IImportService
{
    /// <exception cref="ImportException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="ArgumentNullException"/>
    Task<Statement> ImportStatement(ImportStatementModel request, CancellationToken cancellationToken);
}
