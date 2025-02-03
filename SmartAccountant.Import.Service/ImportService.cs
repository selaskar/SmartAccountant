using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Import.Service.Helpers;
using SmartAccountant.Import.Service.Resources;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service;

public sealed partial class ImportService(ILogger<ImportService> logger, IStorageService storageService, IValidator<ImportStatementModel> validator)
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

        try
        {
            if (!await FileTypeValidator.IsValidFile(request.File, cancellationToken))
                throw new ImportException(Messages.UploadedStatementFileTypeNotSupported);

            Guid documentId = await SaveFile(request.AccountId, request.File, cancellationToken);

            return new Statement()
            {
                Id = Guid.NewGuid(),
                Account = request.AccountId,
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not ImportException)
        {
            UploadFailed(ex, request.AccountId);

            throw new ImportException(Messages.CannotSaveUploadedStatementFile, ex);
        }
    }

    /// <exception cref="StorageException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task<Guid> SaveFile(Guid accountId, ImportFile file, CancellationToken cancellationToken)
    {
        UploadStarting();

        var documentId = Guid.NewGuid();
        string path = $"{AccountsFolderName}/{accountId:D}/{DateTimeOffset.UtcNow.ToString(@"yyyy/MM", CultureInfo.InvariantCulture)}/{documentId:D}";

        using Stream readStream = file.OpenReadStream();
        await storageService.WriteToFile(UploadsContainerName, path, readStream, cancellationToken);

        UploadSucceeded();

        return documentId;
    }


    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded file.")]
    private partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement file successfully uploaded.")]
    private partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while saving the uploaded file for account ({AccountId}).")]
    private partial void UploadFailed(Exception ex, Guid accountId);
}
