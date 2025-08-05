using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPILevel4.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationServices _authenticationServices;
    
    public AuthenticationController(IAuthenticationServices authenticationServices)
    {
        _authenticationServices = authenticationServices;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="login">The request payload containing username and password for login.</param>
    /// <returns>
    /// Returns an ApiResponse containing a JWT token if authentication is successful
    /// </returns>
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> Login([FromBody]LoginRequest login)
    {
        var account = await _authenticationServices.GetToken(login);
        
        return Ok(account);
    }
}