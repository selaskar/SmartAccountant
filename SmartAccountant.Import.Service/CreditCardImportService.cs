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
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IValidator<CreditCardStatementImportModel> validator,
    IStatementFactory statementFactory,
    ISpreadsheetParser parser)
    : AbstractImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, transactionRepository, statementRepository)
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

        var groupedExisting = existingTransactions.GroupBy(x => new { x.Timestamp, x.Description, x.Amount })
            .ToDictionary(x => x.Key, grp => grp.ToArray());

        var groupedNew = creditCardStatement.Transactions.GroupBy(x => new { x.Timestamp, x.Description, x.Amount })
            .ToDictionary(x => x.Key, grp => grp.ToList());

        foreach (var key in groupedNew.Keys.Intersect(groupedExisting.Keys))
            groupedNew[key].RemoveRange(0, groupedExisting[key].Length);

        return [.. groupedNew.SelectMany(x => x.Value)];
    }

    /// <inheritdoc/>
    protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
    {
        CreditCardStatement creditCardStatement = Cast(statement);

        var groupedExisting = existingTransactions.OfType<CreditCardTransaction>()
            .Where(x => x.ProvisionState == ProvisionState.Open)
            .GroupBy(x => new { x.Timestamp, x.Description, x.Amount })
            .ToDictionary(x => x.Key, grp => grp.ToList());

        var groupedNew = creditCardStatement.Transactions.GroupBy(x => new { x.Timestamp, x.Description, x.Amount })
            .ToDictionary(x => x.Key, grp => grp.ToArray());

        foreach (var key in groupedExisting.Keys.Intersect(groupedNew.Keys))
            groupedExisting[key].RemoveRange(0, groupedNew[key].Length);
     
        //The transactions which no longer exist as open transactions in new statement.
        return [.. groupedExisting.SelectMany(x => x.Value)];
    }

    /// <exception cref="ImportException"/>
    private static CreditCardStatement Cast(Statement statement)
    {
        return statement as CreditCardStatement ??
            throw new ImportException($"Statement (type: {statement.GetType().Name}) is expected to be type of {typeof(CreditCardStatement).Name}");
    }
}
