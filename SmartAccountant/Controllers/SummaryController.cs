using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;

namespace SmartAccountant.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class SummaryController(ISummaryService summaryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonthlySummary>> Get([FromQuery] DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            return await summaryService.GetSummary(month, cancellationToken);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
