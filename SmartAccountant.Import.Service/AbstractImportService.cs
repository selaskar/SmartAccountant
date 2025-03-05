using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
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
    ITransactionRepository transactionRepository,
    IStatementRepository statementRepository)
    : IImportService
{
    /// <remarks>In bytes</remarks>
    internal const long MaxFileSize = 1024 * 1024;

    private const string UploadsContainerName = "uploads";
    private const string AccountsFolderName = "accounts";


    /// <inheritdoc/>
    public async Task<Statement> ImportStatement(AbstractStatementImportModel request, CancellationToken cancellationToken)
    {
        Validate(request);

        if (!await fileTypeValidator.IsValidFile(request.File, cancellationToken))
            throw new ImportException(Messages.UploadedStatementFileTypeNotSupported);

        Guid? userId = authorizationService.UserId
            ?? throw new ImportException(Messages.UserNotAuthenticated);

        Account account = ValidateAccountHolder(userId.Value, request.AccountId, cancellationToken);

        Statement statement = Parse(request, account);

        await SaveFile(statement, request.File, cancellationToken);

        Transaction[] existingTransactions = await transactionRepository.GetTransactionsOfAccount(account.Id, cancellationToken);

        Transaction[] newTransactions = DetectExisting(statement, existingTransactions);

        Transaction[] finalizedProvisions = DetectFinalized(statement, existingTransactions);

        await PersistStatement(statement, newTransactions, finalizedProvisions, cancellationToken);

        return statement;
    }


    /// <exception cref="ValidationException"/>
    protected internal abstract void Validate(AbstractStatementImportModel model);

    /// <exception cref="ImportException"/>
    protected internal abstract Statement Parse(AbstractStatementImportModel model, Account account);

    /// <returns>Unique transactions</returns>
    /// <exception cref="ImportException"/>
    protected internal abstract Transaction[] DetectExisting(Statement statement, Transaction[] existingTransactions);

    /// <returns>Returns the transactions that previously existed as open provisions, but became finalized since then.</returns>
    /// <exception cref="ImportException"/>
    protected internal abstract Transaction[] DetectFinalized(Statement statement, Transaction[] existingTransactions);


    /// <exception cref="ImportException" />
    /// <exception cref="OperationCanceledException" />
    private Account ValidateAccountHolder(Guid userId, Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            return accountRepository.GetAccountsOfUser(userId)
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

    /// <remarks>Leaves the file stream in the beginning position.</remarks>
    /// <exception cref="ImportException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task SaveFile(Statement statement, ImportFile file, CancellationToken cancellationToken)
    {
        try
        {
            UploadStarting();

            var documentId = Guid.NewGuid();
            string path = $"{AccountsFolderName}/{statement.AccountId:D}/{DateTimeOffset.UtcNow.ToString(@"yyyy/MM", CultureInfo.InvariantCulture)}/{documentId:D}";

            using Stream readStream = file.OpenReadStream();
            await storageService.WriteToFile(UploadsContainerName, path, readStream, cancellationToken);

            readStream.Seek(0, SeekOrigin.Begin);

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
    private async Task PersistStatement(Statement statement, Transaction[] newTransactions, Transaction[] finalizedTransactions, CancellationToken cancellationToken)
    {
        try
        {
            //Removing finalized transactions, as they no longer exist as open provisions.
            //Since usually their description changes during finalization,
            //it is safer to remove them rather than trying to update them.

            //TODO: do as a single transaction. Unit of work.
            await statementRepository.Insert(statement, cancellationToken);
            await transactionRepository.Insert(newTransactions, cancellationToken);
            await transactionRepository.Delete(finalizedTransactions, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            PersistFailed(ex, statement.AccountId);

            throw new ImportException(Messages.CannotSaveImportedStatement, ex);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while verifying the holder of account ({AccountId}).")]
    protected partial void AccountHolderVerificationFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Parsing of statement document failed for account ({AccountId}).")]
    protected partial void ParseFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded document.")]
    protected partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement document successfully uploaded.")]
    protected partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while saving the uploaded document for account ({AccountId}).")]
    protected partial void UploadFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Persisting of uploaded statement failed for account ({AccountId}).")]
    protected partial void PersistFailed(Exception ex, Guid accountId);
}
