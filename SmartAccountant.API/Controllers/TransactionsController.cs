using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Dtos;
using SmartAccountant.Dtos.Response;

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
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Get([FromQuery] Guid accountId, CancellationToken cancellationToken)
    {
        Models.Transaction[] transactions = await transactionService.GetTransactions(accountId, cancellationToken);
        return Ok(mapper.Map<Transaction[]>(transactions));
    }

    [EndpointSummary("Updates a debit transaction.")]
    [HttpPut("debit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Update([FromBody] DebitTransaction updateDto, CancellationToken cancellationToken)
    {
        var transactionToUpdate = mapper.Map<Models.DebitTransaction>(updateDto);
        await transactionService.UpdateTransaction(transactionToUpdate, cancellationToken);
        return Ok();
    }

    [EndpointSummary("Updates a credit card transaction.")]
    [HttpPut("cc")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction[]>> Update([FromBody] CreditCardTransaction updateDto, CancellationToken cancellationToken)
    {
        var transactionToUpdate = mapper.Map<Models.CreditCardTransaction>(updateDto);
        await transactionService.UpdateTransaction(transactionToUpdate, cancellationToken);
        return Ok();
    }
}
