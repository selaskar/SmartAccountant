using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;

namespace SmartAccountant.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed class SummaryController(ISummaryService summaryService) : ControllerBase
{
    [EndpointSummary("Calculates the summary of given month on currency basis.")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonthlySummary>> Get([FromQuery] DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: need Ok()
            return await summaryService.GetSummary(month, cancellationToken);
        }
        catch (SummaryException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
