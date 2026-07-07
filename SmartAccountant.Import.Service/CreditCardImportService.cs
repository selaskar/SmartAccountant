using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service;

internal sealed class CreditCardImportService(
    ILogger<CreditCardImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IDateTimeService dateTimeService,
    IValidator<CreditCardStatementImportModel> validator,
    IStatementFactory statementFactory,
    IStatementParser parser)
    : AbstractCreditCardImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService, statementFactory)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe(model as CreditCardStatementImportModel);
    }

    public override void Parse(IStatement<CreditCardTransaction> statement)
    {
        parser.ReadStatement(statement);
    }

    //TODO: can be moved to upper class?
    /// <inheritdoc/>
    protected internal override Task PostParse(IStatement<CreditCardTransaction> statement, CancellationToken _)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Transaction[] DetectNew(IStatement<CreditCardTransaction> statement, Transaction[] existingTransactions)
    {
        var creditCardStatement = Cast<CreditCardStatement>(statement);

        return Except(newOnes: creditCardStatement.Transactions, existing: existingTransactions.OfType<CreditCardTransaction>());
    }

    /// <inheritdoc/>
    public override Transaction[] DetectFinalized(IStatement<CreditCardTransaction> statement, Transaction[] existingTransactions)
    {
        var newOpenProvision = statement.Transactions.Where(x => x.ProvisionState == ProvisionState.Open);

        var existingOpenProvision = existingTransactions.OfType<CreditCardTransaction>()
             .Where(x => x.ProvisionState == ProvisionState.Open);

        //The transactions which no longer exist as open transactions in new statement.
        return Except(newOnes: existingOpenProvision, existing: newOpenProvision);
    }
}
