using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed partial class ImportStatementController(IImportService importService) : ControllerBase
{
    [EndpointSummary("Allows importing external statement reports to an account.")]
    [HttpPost("[action]")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Statement>> Upload(
        [FromForm] Guid accountId,
        [Required] IFormFile file,
        [FromForm] DateTimeOffset periodStart,
        [FromForm] DateTimeOffset periodEnd,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        ImportStatementModel requestModel = new ImportStatementModel
        {
            AccountId = accountId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            File = new ImportFile()
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                OpenReadStream = file.OpenReadStream
            },
        };

        try
        {
            return await importService.ImportStatement(requestModel, cancellationToken);
        }
        catch (ImportException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
