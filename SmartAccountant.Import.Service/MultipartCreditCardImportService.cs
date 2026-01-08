using System.Text;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Helpers;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Import.Service;

internal sealed class MultipartCreditCardImportService(
    ILogger<AbstractImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IDateTimeService dateTimeService,
    IValidator<MultipartStatementImportModel> validator,
    IStatementFactory statementFactory,
    IMultipartStatementParser parser)
    : AbstractCreditCardImportService(logger, fileTypeValidator, authorizationService, accountRepository, storageService, unitOfWork, transactionRepository, statementRepository, dateTimeService)
{
    private static readonly CompositeFormat DiscoveredCardNumbersMismatch = CompositeFormat.Parse(Messages.DiscoveredCardNumbersMismatch);
    private static readonly CompositeFormat CannotDetermineSecondaryAccount = CompositeFormat.Parse(Messages.CannotDetermineSecondaryAccount);

    /// <inheritdoc />
    protected internal override void Validate(AbstractStatementImportModel model)
    {
        validator.ValidateAndThrowSafe((MultipartStatementImportModel)model);
    }

    /// <inheritdoc />
    protected internal override async Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken cancellationToken)
    {
        try
        {
            var statement = (SharedStatement)statementFactory.Create(model, account);

            parser.ReadMultipartStatement(statement, model.File.OpenReadStream(), account.Bank);

            await AssignAccountIds(statement, account, cancellationToken);

            return statement;
        }
        catch (ParserException ex)
        {
            throw new ImportException(ImportErrors.CannotParseUploadedStatementFile, ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException and not ImportException)
        {
            throw new ServerException(CannotParseUploadedStatementFile.FormatMessage(account.Id), ex);
        }
    }

    /// <inheritdoc />
    protected internal override async Task<Transaction[]> FetchExistingTransactions(Statement statement, CancellationToken cancellationToken)
    {
        var sharedStatement = Cast<SharedStatement>(statement);

        try
        {
            Transaction[] existingPrimaryTransactions = await TransactionRepository.GetTransactionsOfAccount(sharedStatement.AccountId, cancellationToken);
            Transaction[] existingSecondaryTransactions = await TransactionRepository.GetTransactionsOfAccount(sharedStatement.DependentAccountId!.Value, cancellationToken);
            return existingPrimaryTransactions.Union(existingSecondaryTransactions).ToArray();
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new ServerException(CannotCheckExistingTransactions.FormatMessage(statement.AccountId), ex);
        }
    }

    /// <inheritdoc />
    protected internal override Transaction[] DetectNew(Statement statement, Transaction[] existingTransactions)
    {
        var sharedStatement = Cast<SharedStatement>(statement);

        var combinedTransactions = sharedStatement.Transactions.Union(sharedStatement.SecondaryTransactions);

        return Except(news: combinedTransactions, existing: existingTransactions.OfType<CreditCardTransaction>());
    }

    /// <inheritdoc />
    protected internal override Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions)
    {
        //Open provisions don't apply to multipart statements.
        return [];
    }


    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private async Task AssignAccountIds(SharedStatement statement, Account primaryAccount, CancellationToken cancellationToken)
    {
        var abstractPrimaryCreditCard = primaryAccount as AbstractCreditCard
            ?? throw new ImportException(ImportErrors.AbstractCreditCardExpected ,$"Primary account (type: {primaryAccount.GetType().Name}) is expected to be type of {typeof(AbstractCreditCard).Name}");

        string cardNumberToSearch;
        bool transactionsInCorrectOrder;
        if (CreditCardUtilities.CompareNumbersWithMasking(abstractPrimaryCreditCard.CardNumber, statement.CardNumber1))
        {
            cardNumberToSearch = statement.CardNumber2
                ?? throw new ImportException(ImportErrors.SecondaryCardNumberNotDetermined);

            transactionsInCorrectOrder = true;           
        }
        else if (CreditCardUtilities.CompareNumbersWithMasking(abstractPrimaryCreditCard.CardNumber, statement.CardNumber2))
        {
            cardNumberToSearch = statement.CardNumber1
                ?? throw new ImportException(ImportErrors.PrimaryCardNumberNotDetermined);

            transactionsInCorrectOrder = false;
        }
        else
            throw new ImportException(ImportErrors.DiscoveredCardNumbersMismatch, DiscoveredCardNumbersMismatch.FormatMessage(statement.CardNumber1, statement.CardNumber2, abstractPrimaryCreditCard.CardNumber));

        Account[] accounts = await AccountRepository.GetAccountsOfUser(primaryAccount.HolderId, cancellationToken);

        foreach (Account secondaryAccount in accounts)
        {
            if (secondaryAccount.Id == abstractPrimaryCreditCard.Id)
                continue;

            if (secondaryAccount is not AbstractCreditCard abstractSecondaryCreditCard)
                continue;

            if (CreditCardUtilities.CompareNumbersWithMasking(abstractSecondaryCreditCard.CardNumber, cardNumberToSearch))
            {
                statement.DependentAccountId = abstractSecondaryCreditCard.Id;
                break;
            }
        }

        if (statement.DependentAccountId is null)
            throw new ImportException(ImportErrors.CannotDetermineSecondaryAccount, CannotDetermineSecondaryAccount.FormatMessage(cardNumberToSearch));

        if (transactionsInCorrectOrder)
        {
            foreach (Transaction primaryTransaction in statement.Transactions)
                primaryTransaction.AccountId = statement.AccountId;

            foreach (Transaction secondaryTransaction in statement.SecondaryTransactions)
                secondaryTransaction.AccountId = statement.DependentAccountId;
        }
        else
        {
            foreach (Transaction primaryTransaction in statement.Transactions)
                primaryTransaction.AccountId = statement.DependentAccountId;

            foreach (Transaction secondaryTransaction in statement.SecondaryTransactions)
                secondaryTransaction.AccountId = statement.AccountId;
        }
    }
}
