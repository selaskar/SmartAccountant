using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Models.Response;

namespace SmartAccountant.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed partial class ImportStatementController(IImportService importService, IMapper mapper) : ControllerBase
{
    [EndpointSummary("Allows importing external statement reports to a debit account.")]
    [HttpPost("debit")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadDebit(
        [FromForm] UploadDebitStatementRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestModel = mapper.Map<DebitStatementImportModel>(request);

        try
        {
            Statement statement = await importService.ImportDebitStatement(requestModel, cancellationToken);

            UploadStatementResponse response = mapper.Map<UploadStatementResponse>(statement) with
            {
                RequestId = request.RequestId
            };

            return Ok(response);
        }
        catch (ImportException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }

    /*
    [EndpointSummary("Allows importing external statement reports for a credit card.")]
    [HttpPost("CreditCard")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadCreditCard(
        [FromForm] UploadCreditCardStatementRequest request, CancellationToken cancellationToken)
    {
        // TODO: move to common method
        ArgumentNullException.ThrowIfNull(request);

        var requestModel = mapper.Map<CreditCardStatementImportModel>(request);

        try
        {
            Statement statement = await importService.ImportCreditCardStatement(requestModel, cancellationToken);

            UploadStatementResponse response = mapper.Map<UploadStatementResponse>(statement) with
            {
                RequestId = request.RequestId
            };

            return Ok(response);
        }
        catch (ImportException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }*/
}
