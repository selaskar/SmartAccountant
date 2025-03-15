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
public sealed class ImportStatementController(IMapper mapper) : ControllerBase
{
    [EndpointSummary("Allows importing external statement reports to a debit account.")]
    [HttpPost(nameof(ImportableStatementTypes.Debit))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadDebit(
        [FromKeyedServices(nameof(ImportableStatementTypes.Debit))] IImportService importService,
        [FromForm] UploadDebitStatementRequest request,
        CancellationToken cancellationToken)
    {
        return await ImportInternal<DebitStatementImportModel>(importService, request, cancellationToken);
    }


    [EndpointSummary("Allows importing external statement reports for a credit card.")]
    [HttpPost(nameof(ImportableStatementTypes.CreditCard))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadCreditCard(
        [FromKeyedServices(nameof(ImportableStatementTypes.CreditCard))] IImportService importService,
        [FromForm] UploadCreditCardStatementRequest request,
        CancellationToken cancellationToken)
    {
        return await ImportInternal<CreditCardStatementImportModel>(importService, request, cancellationToken);
    }

    private async Task<ActionResult<UploadStatementResponse>> ImportInternal<TModel>(
        IImportService importService,
        AbstractUploadStatementRequest request,
        CancellationToken cancellationToken)
        where TModel : AbstractStatementImportModel
    {
        ArgumentNullException.ThrowIfNull(importService);
        ArgumentNullException.ThrowIfNull(request);

        var requestModel = mapper.Map<TModel>(request);

        try
        {
            Statement statement = await importService.ImportStatement(requestModel, cancellationToken);

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
}
