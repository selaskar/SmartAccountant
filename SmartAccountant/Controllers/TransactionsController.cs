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
public sealed class TransactionsController(ITransactionService transactionService) : ControllerBase
{
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
