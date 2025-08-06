using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPILevel4.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserServices _service;

    public UserController(IUserServices service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Registers a new user account and returns the new account details.
    /// </summary>
    /// <param name="request">The request payload containing user registration details such as name, username, password, and role.</param>
    /// <returns>
    /// Returns new account details.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var response = await _service.CreateAccount(request);

        if (!response.IsSuccess)
        {
            return StatusCode(response.HttpStatusCode, response.ErrorMessage);
        }
        
        var createdAccount = response.Result;

        return CreatedAtRoute(
            routeName: "GetAccountById",                  
            routeValues: new { accountNumber = createdAccount.AccountNumber }, 
            value: createdAccount
        );
    }
}