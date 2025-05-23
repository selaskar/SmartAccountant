﻿using FluentValidation;
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

internal sealed class DebitImportService(
    ILogger<AbstractImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IValidator<DebitStatementImportModel> validator,
    IStatementFactory statementFactory,
    ISpreadsheetParser parser)
    : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((DebitStatementImportModel)model);
    }

    /// <inheritdoc/>
    protected internal override Statement Parse(AbstractStatementImportModel model, Account account)
    {
        try
        {
            var debitStatementImportModel = model as DebitStatementImportModel ??
                throw new ImportException($"Model (type: {model.GetType().Name}) is expected to be type of {typeof(DebitStatementImportModel).Name}");

            var statement = (DebitStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

            return statement;
        }
        catch (Exception ex) when (ex is not ImportException)
        {
            ParseFailed(ex, account.Id);

            throw new ImportException(Messages.CannotParseUploadedStatementFile, ex);
        }
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectExisting(Statement statement, Transaction[] existingTransactions)
    {
        var debitStatement = statement as DebitStatement ??
            throw new ImportException($"Statement (type: {statement.GetType().Name}) is expected to be type of {typeof(DebitStatement).Name}");

        //TODO: ref number nullable
        var existingIdentifiers = existingTransactions.OfType<DebitTransaction>().Select(x => new { x.ReferenceNumber, x.RemainingBalance });
        IEnumerable<DebitTransaction> newTransactions = debitStatement.Transactions
            .ExceptBy(existingIdentifiers, x => new { x.ReferenceNumber, x.RemainingBalance });

        return [.. newTransactions];
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
    {
        //Open provisions don't apply to debit accounts.
        return [];
    }
}
