using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Resources;

namespace SmartAccountant.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed partial class ImportStatementController(ILogger<ImportStatementController> logger, IStorageService storageService)
    : ControllerBase
{
    private const string UploadsFolderName = "uploads";

    private const long MaxFileSize = 1024 * 1024; // 1 MB

    [EndpointSummary("Allows importing external statement reports to an account. Maximum allowed file size is 1 MB.")]
    [HttpPost("[action]")]
    [Produces<Guid>]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] Guid accountId, [Required] IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(Messages.UploadedStatementFileEmpty);

        if (file.Length > MaxFileSize)
            return BadRequest(Messages.UploadedStatementFileTooBig);

        if (file is FormFile formFile && formFile.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            return BadRequest(Messages.UploadedStatementFileTypeNotSupported);

        //TODO: virus scan


        UploadStarting();
        try
        {
            Guid documentId = await SaveFile(accountId, file, cancellationToken);

            UploadSucceeded();

            return Ok(documentId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            UploadFailed(ex, accountId);

            return Problem(Messages.CannotSaveUploadedStatementFile, statusCode: StatusCodes.Status500InternalServerError);
        }

    }

    /// <exception cref="StorageException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task<Guid> SaveFile(Guid accountId, IFormFile file, CancellationToken cancellationToken)
    {
        var documentId = Guid.NewGuid();
        string path = $"accounts/{accountId:D}/{DateTimeOffset.UtcNow.ToString(@"yyyy/MM", CultureInfo.InvariantCulture)}/{documentId:D}";

        using Stream readStream = file.OpenReadStream();
        await storageService.WriteToFile(UploadsFolderName, path, readStream, cancellationToken);

        return documentId;
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting to save the uploaded file.")]
    private partial void UploadStarting();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Statement file successfully uploaded.")]
    private partial void UploadSucceeded();

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while saving the uploaded file for account ({AccountId}).")]
    private partial void UploadFailed(Exception ex, Guid accountId);
}
