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
public sealed class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<BadRequestObjectResult>(StatusCodes.Status400BadRequest)]
    public ActionResult<IAsyncEnumerable<Account>> Get()
    {
        try
        {
            return Ok(accountService.GetAccountsOfUser());
        }
        catch (AccountException ex)
        {
            return BadRequest(ex.GetAllMessages());
        }
    }
}
