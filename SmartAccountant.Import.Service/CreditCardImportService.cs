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
    ILogger<AbstractImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IValidator<CreditCardStatementImportModel> validator,
    IStatementFactory statementFactory,
    ISpreadsheetParser parser)
    : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository)
{
    /// <inheritdoc/>
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((CreditCardStatementImportModel)model);
    }

    /// <inheritdoc/>
    protected internal override Statement Parse(AbstractStatementImportModel model, Account account)
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

    /// <inheritdoc/>
    protected internal override Transaction[] DetectExisting(Statement statement, Transaction[] existingTransactions)
    {
        CreditCardStatement creditCardStatement = Cast(statement);

        return Except(news: creditCardStatement.Transactions, existing: existingTransactions.OfType<CreditCardTransaction>());
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
    {
        CreditCardStatement creditCardStatement = Cast(statement);

        var newOpenProvision = creditCardStatement.Transactions.Where(x => x.ProvisionState == ProvisionState.Open);

        var existingOpenProvision = existingTransactions.OfType<CreditCardTransaction>()
             .Where(x => x.ProvisionState == ProvisionState.Open);

        //The transactions which no longer exist as open transactions in new statement.
        return Except(news: existingOpenProvision, existing: newOpenProvision);
    }

    /// <exception cref="ImportException"/>
    private static CreditCardStatement Cast(Statement statement)
    {
        return statement as CreditCardStatement ??
            throw new ImportException($"Statement (type: {statement.GetType().Name}) is expected to be type of {typeof(CreditCardStatement).Name}");
    }

    private static Transaction[] Except(IEnumerable<CreditCardTransaction> news, IEnumerable<CreditCardTransaction> existing)
    {
        var groupedExisting = existing.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToArray());

        var groupedNew = news.GroupBy(x => new { x.Timestamp, x.Description, x.Amount, x.ProvisionState })
            .ToDictionary(x => x.Key, grp => grp.ToList());

        foreach (var key in groupedNew.Keys.Intersect(groupedExisting.Keys))
            groupedNew[key].RemoveRange(0, groupedExisting[key].Length);

        return [.. groupedNew.SelectMany(x => x.Value)];
    }
}
