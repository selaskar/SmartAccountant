using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Dtos;

namespace SmartAccountant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
public sealed class TransactionsController(ITransactionService transactionService, IMapper mapper) : ControllerBase
{
    [EndpointSummary("Returns all transactions of given account.")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Get([FromQuery] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            Models.Transaction[] transactions = await transactionService.GetTransactions(accountId, cancellationToken);
            return Ok(mapper.Map<Transaction[]>(transactions));
        }
        catch (TransactionException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }

    [EndpointSummary("Updates a debit transaction.")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Update([FromBody] DebitTransaction updateDto, CancellationToken cancellationToken)
    {
        try
        {
            var transactionToUpdate = mapper.Map<Models.DebitTransaction>(updateDto);
            await transactionService.UpdateTransaction(transactionToUpdate, cancellationToken);
            return Ok();
        }
        catch (TransactionException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
