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
public sealed class AccountsController(IAccountService accountService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetail>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Account[]>> Get(CancellationToken cancellationToken)
    {
        Models.Account[] accounts = await accountService.GetAccountsOfUser(cancellationToken);
        return Ok(accounts.Select(mapper.Map<Account>));
    }
}
