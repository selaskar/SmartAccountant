using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service;

internal sealed class DebitImportService(
    ILogger<DebitImportService> logger, //TODO: how does this change affect the printed logs?
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IDateTimeService dateTimeService,
    IStatementFactory statementFactory, //TODO: move up?
    IStatementParser parser,
    IValidator<DebitStatementImportModel> validator)
    : AbstractImportService<DebitTransaction>(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService, statementFactory)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe(model as DebitStatementImportModel);
    }

    public override void Parse(IStatement<DebitTransaction> statement, CancellationToken cancellationToken)
    {
        parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);
    }

    /// <inheritdoc/>
    protected internal override Task PostParse(IStatement<DebitTransaction> statement, CancellationToken _)
    {
        DebitTransaction? lastTransaction = statement.Transactions.LastOrDefault();

        var debitStatement = Cast<DebitStatement>(statement);

        debitStatement.RemainingBalance = lastTransaction?.RemainingBalance.Amount ?? 0;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override DebitTransaction[] DetectNew(IStatement<DebitTransaction> statement, Transaction[] existingTransactions)
    {
        //TODO: ref number nullable
        var existingIdentifiers = existingTransactions.OfType<DebitTransaction>().Select(x => new { x.ReferenceNumber, x.RemainingBalance });
        IEnumerable<DebitTransaction> newTransactions = statement.Transactions
            .ExceptBy(existingIdentifiers, x => new { x.ReferenceNumber, x.RemainingBalance });

        return [.. newTransactions];
    }

    /// <inheritdoc/>
    public override DebitTransaction[] DetectFinalized(IStatement<DebitTransaction> statement, Transaction[] existingTransactions)
    {
        //Open provisions don't apply to debit accounts.
        return [];
    }
}
