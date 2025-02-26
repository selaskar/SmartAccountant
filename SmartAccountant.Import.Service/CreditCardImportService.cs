using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service;

internal sealed class CreditCardImportService(
    ILogger<ImportService> logger,
    IValidator<CreditCardStatementImportModel> validator,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStatementFactory statementFactory,
    ISpreadsheetParser parser,
    IStorageService storageService,
    IStatementRepository statementRepository) :
        ImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, statementRepository)
{
    /// <inheritdoc/>
    protected override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((CreditCardStatementImportModel)model);
    }

    /// <inheritdoc/>
    protected override Statement Parse(AbstractStatementImportModel model, Account account)
    {
        try
        {
            var creditCardStatementImportModel = model as CreditCardStatementImportModel ??
                throw new ImportException($"Model (type: {model.GetType().Name}) is expected to be type of {typeof(CreditCardStatementImportModel).Name}");

            var statement = (CreditCardStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

            return statement;
        }
        catch (Exception ex) when (ex is not ImportException)
        {
            ParseFailed(ex, account.Id);

            throw new ImportException(Messages.CannotParseUploadedStatementFile, ex);
        }
    }
}
