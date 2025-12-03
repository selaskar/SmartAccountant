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
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Import.Service;

internal sealed class CreditCardImportService(
    ILogger<AbstractImportService> logger,
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
    : AbstractCreditCardImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((CreditCardStatementImportModel)model);
    }

    /// <inheritdoc/>
    protected internal override Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken _)
    {
        try
        {
            var statement = (CreditCardStatement)statementFactory.Create(model, account);

            parser.ReadStatement(statement, model.File.OpenReadStream(), account.Bank);

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
        var creditCardStatement = Cast<CreditCardStatement>(statement);

        return Except(news: creditCardStatement.Transactions, existing: existingTransactions.OfType<CreditCardTransaction>());
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
    {
        var creditCardStatement = Cast<CreditCardStatement>(statement);

        var newOpenProvision = creditCardStatement.Transactions.Where(x => x.ProvisionState == ProvisionState.Open);

        var existingOpenProvision = existingTransactions.OfType<CreditCardTransaction>()
             .Where(x => x.ProvisionState == ProvisionState.Open);

        //The transactions which no longer exist as open transactions in new statement.
        return Except(news: existingOpenProvision, existing: newOpenProvision);
    }

    /// <inheritdoc/>
    protected internal override Balance CalculateRemaining(Statement statement)
    {
        var creditCardStatement = Cast<CreditCardStatement>(statement);

        //(creditCardStatement.Account as CreditCard).GetLimit();
        //creditCardStatement.TotalDueAmount

        return null;
    }
}
