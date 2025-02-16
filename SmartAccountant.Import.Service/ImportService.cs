using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Helpers;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;

namespace SmartAccountant.Import.Service;

internal sealed partial class ImportService(
    ILogger<ImportService> logger,
    IValidator<ImportStatementModel> validator,
    IStorageService storageService,
    IAuthorizationService authorizationService,
    IAccountRepository accountRepository,
    IStatementFactory statementFactory,
    ISpreadsheetParser parser,
    IStatementRepository statementRepository)
    : IImportService
{
    /// <remarks>In bytes</remarks>
    internal const long MaxFileSize = 1024 * 1024;

    private const string UploadsContainerName = "uploads";
    private const string AccountsFolderName = "accounts";

    /// <inheritdoc/>
    public async Task<Statement> ImportStatement(ImportStatementModel request, CancellationToken cancellationToken)
    {
        validator.ValidateAndThrowSafe(request);

        if (!await FileTypeValidator.IsValidFile(request.File, cancellationToken))
            throw new ImportException(Messages.UploadedStatementFileTypeNotSupported);

        Guid? userId = authorizationService.UserId
            ?? throw new ImportException(Messages.UserNotAuthenticated);

        Account account = ValidateAccountHolder(userId.Value, request.AccountId, cancellationToken);

        Statement statement = Parse(request, account);

        await SaveFile(statement, request.File, cancellationToken);

        return await PersistStatement(statement, cancellationToken);
    }

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

    /// <exception cref="ImportException"/>
    private Statement Parse(ImportStatementModel request, Account account)
    {
        try
        {
            Statement statement = statementFactory.Create(request, account);

            parser.ReadStatement((Statement<DebitTransaction>)statement, request.File.OpenReadStream());
            
            return statement;
        }
        catch (Exception ex)
        {
            ParseFailed(ex, account.Id);

            throw new ImportException(Messages.CannotParseUploadedStatementFile, ex);
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
    private async Task<Statement> PersistStatement(Statement statement, CancellationToken cancellationToken)
    {
        try
        {
            await statementRepository.Insert(statement, cancellationToken);

            return statement;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            PersistFailed(ex, statement.AccountId);

            throw new ImportException(Messages.CannotSaveImportedStatement, ex);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while verifying the holder of account ({AccountId}).")]
    private partial void AccountHolderVerificationFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Parsing of statement document failed for account ({AccountId}).")]
    private partial void ParseFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded document.")]
    private partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement document successfully uploaded.")]
    private partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while saving the uploaded document for account ({AccountId}).")]
    private partial void UploadFailed(Exception ex, Guid accountId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Persisting of uploaded statement failed for account ({AccountId}).")]
    private partial void PersistFailed(Exception ex, Guid accountId);
}
