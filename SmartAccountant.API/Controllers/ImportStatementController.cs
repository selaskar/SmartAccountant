using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Dtos.Request;
using SmartAccountant.Dtos.Response;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;

namespace SmartAccountant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed class ImportStatementController(IMapper mapper) : ControllerBase
{
    [EndpointSummary("Allows importing external statement reports to a debit account.")]
    [HttpPost(nameof(ImportableStatementTypes.Debit))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadCreditCard(
        [FromKeyedServices(nameof(ImportableStatementTypes.CreditCard))] IImportService importService,
        [FromForm] UploadCreditCardStatementRequest request,
        CancellationToken cancellationToken)
    {
        return await ImportInternal<CreditCardStatementImportModel>(importService, request, cancellationToken);
    }

    [EndpointSummary("Allows importing external statement reports which are consist of transactions from multiple dependent accounts.")]
    [HttpPost(nameof(ImportableStatementTypes.Multipart))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> UploadMultipartCreditCard(
        [FromKeyedServices(nameof(ImportableStatementTypes.Multipart))] IImportService importService,
        [FromForm] UploadMultipartStatementRequest request,
        CancellationToken cancellationToken)
    {
        return await ImportInternal<MultipartStatementImportModel>(importService, request, cancellationToken);
    }

    /// <exception cref="ImportException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ValidationException"/>
    /// <exception cref="AuthenticationException"/>
    private async Task<ActionResult<UploadStatementResponse>> ImportInternal<TModel>(
        IImportService importService,
        AbstractUploadStatementRequest request,
        CancellationToken cancellationToken)
        where TModel : AbstractStatementImportModel
    {
        var requestModel = mapper.Map<TModel>(request);

        Statement statement = await importService.ImportStatement(requestModel, cancellationToken);

        UploadStatementResponse response = mapper.Map<UploadStatementResponse>(statement) with
        {
            RequestId = request.RequestId
        };

        return Ok(response);
    }
}
