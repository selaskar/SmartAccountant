using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
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
    [EndpointSummary("Allows importing external statement reports to an account.")]
    [HttpPost("[action]")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadStatementResponse>> Upload([FromForm] UploadStatementRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ImportStatementModel requestModel = mapper.Map<ImportStatementModel>(request);

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
            return BadRequest(ex.Message);
        }
    }
}
