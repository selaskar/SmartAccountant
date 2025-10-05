using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;

namespace SmartAccountant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [EndpointSummary("Returns all transactions of given account.")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Get([FromQuery] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await transactionService.GetTransactions(accountId, cancellationToken));
        }
        catch (TransactionException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
