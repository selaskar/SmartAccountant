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
public sealed class AccountsController(IAccountService accountService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Account[]>> Get(CancellationToken cancellationToken)
    {
        try
        {
            Models.Account[] accounts = await accountService.GetAccountsOfUser(cancellationToken);
            return Ok(accounts.Select(mapper.Map<Account>));
        }
        catch (AccountException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
