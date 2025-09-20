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
    IDateTimeService dateTimeService,
    IValidator<DebitStatementImportModel> validator,
    IStatementFactory statementFactory,
    IStatementParser parser)
    : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((DebitStatementImportModel)model);
    }

    /// <inheritdoc/>
    protected internal override Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken _)
    {
        try
        {
            var statement = (DebitStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

            DebitTransaction? lastTransaction = statement.Transactions.LastOrDefault();

            statement.RemainingBalance = lastTransaction?.RemainingBalance.Amount ?? 0;

            return Task.FromResult<Statement>(statement);
        }
        catch (Exception ex) when (ex is not ImportException)
        {
            ParseFailed(ex, account.Id);

            throw new ImportException(Messages.CannotParseUploadedStatementFile, ex);
        }
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectNew(Statement statement, Transaction[] existingTransactions)
    {
        var debitStatement = Cast<DebitStatement>(statement);

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

    /// <inheritdoc/>
    protected internal override Balance CalculateRemaining(Statement statement)
    {
        var debitStatement = Cast<DebitStatement>(statement);

        var lastTrx = debitStatement.Transactions.LastOrDefault();

        if (lastTrx == null)
        {

        }

        return new Balance()
        {
            Id = Guid.NewGuid(),
            SavingAccountId = statement.AccountId,
            Amount = new MonetaryValue(debitStatement.RemainingBalance, debitStatement.Currency),
            AsOf = lastTrx.Timestamp,
        };
    }
}
