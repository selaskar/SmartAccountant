using System.Globalization;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Shared.Enums.Errors;

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

    private static readonly CompositeFormat CannotImportStatement = CompositeFormat.Parse(Messages.CannotImportStatement);
    private static readonly CompositeFormat CannotValidateAccountHolder = CompositeFormat.Parse(Messages.CannotValidateAccountHolder);
    private static readonly CompositeFormat CannotSaveUploadedStatementFile = CompositeFormat.Parse(Messages.CannotSaveUploadedStatementFile);
    private static readonly CompositeFormat CannotSaveImportingStatement = CompositeFormat.Parse(Messages.CannotSaveImportingStatement);
    private protected static readonly CompositeFormat CannotCheckExistingTransactions = CompositeFormat.Parse(Messages.CannotCheckExistingTransactions);
    private protected static readonly CompositeFormat CannotParseUploadedStatementFile = CompositeFormat.Parse(Messages.CannotParseUploadedStatementFile);

    private protected IAccountRepository AccountRepository { get; } = accountRepository;
    private protected ITransactionRepository TransactionRepository { get; } = transactionRepository;

    /// <inheritdoc/>
    public async Task<Statement> ImportStatement(AbstractStatementImportModel model, CancellationToken cancellationToken)
    {
        Statement? statement = null;

        Validate(model);

        Guid userId = authorizationService.UserId;

        try
        {
            if (!await fileTypeValidator.IsValidFile(model.File, cancellationToken))
                throw new ImportException(ImportErrors.UploadedStatementFileTypeNotSupported);

            Account account = await ValidateAccountHolder(userId, model.AccountId, cancellationToken);

            statement = await Parse(model, account, cancellationToken);

            await SaveFile(statement, model.File, cancellationToken);

            (Transaction[] newTransactions, Transaction[] finalizedProvisions) = await Collide(statement, cancellationToken);

            await PersistStatement(statement, newTransactions, finalizedProvisions, cancellationToken);

            return statement;
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException and not ImportException)
        {
            throw new ServerException(CannotImportStatement.FormatMessage(statement?.Id, model.AccountId), ex);
        }
    }


    /// <exception cref="ValidationException"/>
    protected internal abstract void Validate(AbstractStatementImportModel model);

    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    protected internal abstract Task<Statement> Parse(AbstractStatementImportModel model, Account account, CancellationToken cancellationToken);

    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    protected internal virtual async Task<Transaction[]> FetchExistingTransactions(Statement statement, CancellationToken cancellationToken)
    {
        try
        {
            Transaction[] existingTransactions = await TransactionRepository.GetTransactionsOfAccount(statement.AccountId, cancellationToken);
            return existingTransactions;
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new ServerException(CannotCheckExistingTransactions.FormatMessage(statement.AccountId), ex);
        }
    }

    /// <returns>Unique transactions</returns>
    /// <exception cref="ImportException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected internal abstract Transaction[] DetectNew(Statement statement, Transaction[] existingTransactions);

    /// <returns>Returns the transactions that previously existed as open provisions, but became finalized since then.</returns>
    /// <exception cref="ImportException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    protected internal abstract Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions);

    /// <exception cref="ImportException" />
    /// <exception cref="ServerException" />
    /// <exception cref="OperationCanceledException" />
    private async Task<Account> ValidateAccountHolder(Guid userId, Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            Account[] accounts = await AccountRepository.GetAccountsOfUser(userId, cancellationToken);

            return accounts.FirstOrDefault(x => x.Id == accountId)
               ?? throw new ImportException(ImportErrors.AccountDoesNotBelongToUser); // or does not exist
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException and not ImportException)
        {
            throw new ServerException(CannotValidateAccountHolder.FormatMessage(accountId), ex);
        }
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
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
        catch (StorageException ex)
        {
            throw new ImportException(ImportErrors.CannotSaveUploadedStatementFile, ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ServerException)
        {
            throw new ServerException(CannotSaveUploadedStatementFile.FormatMessage(statement.AccountId), ex);
        }
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private async Task<(Transaction[], Transaction[])> Collide(Statement statement, CancellationToken cancellationToken)
    {
        Transaction[] existingTransactions = await FetchExistingTransactions(statement, cancellationToken);

        Transaction[] newTransactions = DetectNew(statement, existingTransactions);

        Transaction[] finalizedProvisions = DetectFinalized(statement, existingTransactions);

        return (newTransactions, finalizedProvisions);
    }

    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task PersistStatement(Statement statement, Transaction[] newTransactions, Transaction[] finalizedTransactions, CancellationToken cancellationToken)
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

            await unitOfWork.CommitAsync(cancellationToken);

            PersistSucceeded();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await unitOfWork.RollbackAsync(cancellationToken);

            throw new ServerException(CannotSaveImportingStatement.FormatMessage(statement.AccountId), ex);
        }
    }


    /// <exception cref="ImportException"/>
    private protected static TStatement Cast<TStatement>(Statement statement)
        where TStatement : Statement
    {
        return statement as TStatement ??
            throw new ImportException(ImportErrors.StatementTypeMismatch, $"Statement (type: {statement.GetType().Name}) was expected to be type of {typeof(TStatement).Name}");
    }


    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded document.")]
    private partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement document successfully uploaded.")]
    private partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Trace, Message = "The uploaded statement successfully persisted.")]
    private partial void PersistSucceeded();
}
