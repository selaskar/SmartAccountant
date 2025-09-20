using System.Globalization;
using System.Text;
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

internal abstract partial class AbstractImportService(
    ILogger<AbstractImportService> logger,
    IFileTypeValidator fileTypeValidator,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository,
    IDateTimeService dateTimeService)
    : IImportService
{
    /// <remarks>In bytes</remarks>
    internal const long MaxFileSize = 1024 * 1024;

    private const string UploadsContainerName = "uploads";
    private const string AccountsFolderName = "accounts";

    private protected static readonly CompositeFormat CannotCheckExistingTransactions = CompositeFormat.Parse(Messages.CannotCheckExistingTransactions);

    private protected IAccountRepository AccountRepository { get; } = accountRepository;
    private protected ITransactionRepository TransactionRepository { get; } = transactionRepository;

    /// <inheritdoc/>
    public async Task<Statement> ImportStatement(AbstractStatementImportModel model, CancellationToken cancellationToken)
    {
        Validate(model);

        if (!await fileTypeValidator.IsValidFile(model.File, cancellationToken))
            throw new ImportException(Messages.UploadedStatementFileTypeNotSupported);

        Guid userId = authorizationService.UserId;

        Account account = ValidateAccountHolder(userId, model.AccountId, cancellationToken);

        Statement statement = await Parse(model, account, cancellationToken);

        await SaveFile(statement, model.File, cancellationToken);

        (Transaction[] newTransactions, Transaction[] finalizedProvisions) = await Collide(statement, cancellationToken);

        await PersistStatement(statement, newTransactions, finalizedProvisions, balance: null, cancellationToken);

        //Balance remainingBalance = CalculateRemaining(statement);

        return statement;
    }


    /// <exception cref="ValidationException"/>
    protected internal abstract void Validate(AbstractStatementImportModel model);

    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    protected internal abstract Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken cancellationToken);

    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    protected internal virtual async Task<Transaction[]> FetchExistingTransactions(Statement statement, CancellationToken cancellationToken)
    {
        try
        {
            Transaction[] existingTransactions = await TransactionRepository.GetTransactionsOfAccount(statement.AccountId, cancellationToken);
            return existingTransactions;
        }
        catch (RepositoryException ex)
        {
            throw new ImportException(CannotCheckExistingTransactions.FormatMessage(statement.AccountId), ex);
        }
    }

    /// <returns>Unique transactions</returns>
    /// <exception cref="ImportException"/>
    protected internal abstract Transaction[] DetectNew(Statement statement, Transaction[] existingTransactions);

    /// <returns>Returns the transactions that previously existed as open provisions, but became finalized since then.</returns>
    /// <exception cref="ImportException"/>
    protected internal abstract Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions);

    /// <exception cref="ImportException"/>
    protected internal abstract Balance CalculateRemaining(Statement statement);

    /// <exception cref="ImportException" />
    /// <exception cref="OperationCanceledException" />
    private Account ValidateAccountHolder(Guid userId, Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            return AccountRepository.GetAccountsOfUser(userId)
                .ToBlockingEnumerable(cancellationToken)
                .FirstOrDefault(x => x.Id == accountId)
               ?? throw new ImportException(Messages.AccountDoesNotBelongToUser); // or does not exist
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ImportException)
        {
            AccountHolderVerificationFailed(ex, accountId);

            throw new ImportException(Messages.CannotValidateAccountHolder, ex);
        }
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task SaveFile(Statement statement, ImportFile file, CancellationToken cancellationToken)
    {
        try
        {
            UploadStarting();

            var documentId = Guid.NewGuid();
            string path = $"{AccountsFolderName}/{statement.AccountId:D}/{dateTimeService.UtcNow.ToString(@"yyyy/MM", CultureInfo.InvariantCulture)}/{documentId:D}";

            using Stream readStream = file.OpenReadStream();
            await storageService.WriteToFile(UploadsContainerName, path, readStream, cancellationToken);

            UploadSucceeded();

            statement.Documents.Add(new StatementDocument()
            {
                DocumentId = documentId,
                StatementId = statement.Id,
                Statement = statement,
                FilePath = path
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            UploadFailed(ex, statement.AccountId);

            throw new ImportException(Messages.CannotSaveUploadedStatementFile, ex);
        }
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task<(Transaction[], Transaction[])> Collide(Statement statement, CancellationToken cancellationToken)
    {
        Transaction[] existingTransactions = await FetchExistingTransactions(statement, cancellationToken);

        Transaction[] newTransactions = DetectNew(statement, existingTransactions);

        Transaction[] finalizedProvisions = DetectFinalized(statement, existingTransactions);

        return (newTransactions, finalizedProvisions);
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task PersistStatement(Statement statement, Transaction[] newTransactions, Transaction[] finalizedTransactions, Balance? balance, CancellationToken cancellationToken)
    {
        try
        {
            //Removing finalized transactions, as they no longer exist as open provisions.
            //Since usually their description changes during finalization,
            //it is safer to remove them rather than trying to update them.

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            await statementRepository.Insert(statement, cancellationToken);
            await TransactionRepository.Insert(newTransactions, cancellationToken);
            await TransactionRepository.Delete(finalizedTransactions, cancellationToken);

            if (balance != null)
                await AccountRepository.SaveBalance(balance, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

            PersistSucceeded();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            PersistFailed(ex, statement.AccountId);

            await unitOfWork.RollbackAsync(cancellationToken);

            throw new ImportException(Messages.CannotSaveImportedStatement, ex);
        }
    }


    /// <exception cref="ImportException"/>
    private protected static TStatement Cast<TStatement>(Statement statement)
        where TStatement : Statement
    {
        return statement as TStatement ??
            throw new ImportException($"Statement (type: {statement.GetType().Name}) was expected to be type of {typeof(TStatement).Name}");
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while verifying the holder of account ({AccountId}).")]
    private partial void AccountHolderVerificationFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Parsing of statement document failed for account ({AccountId}).")]
    private protected partial void ParseFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded document.")]
    private partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement document successfully uploaded.")]
    private partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while saving the uploaded document for account ({AccountId}).")]
    private protected partial void UploadFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "The uploaded statement successfully persisted.")]
    private partial void PersistSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "Persisting of uploaded statement failed for account ({AccountId}).")]
    private partial void PersistFailed(Exception ex, Guid accountId);
}
