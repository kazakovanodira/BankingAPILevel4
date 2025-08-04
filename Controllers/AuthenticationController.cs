using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using banking_api_repo.Interfaces;
using banking_api_repo.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace banking_api_repo.Controllers;

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
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))] // Or Type = typeof(ServiceResponse<string>) if that's the structure
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var account = await _authenticationServices.CheckIfPasswordsMatchesUsername(login);

        if (account.IsSuccess)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, account.Result.Username),
                new(ClaimTypes.Name, account.Result.Name),
                new(ClaimTypes.Role, account.Result.Role)
            }, "Bearer");

            var token = _authenticationServices.CreateSecurityToken(claimsIdentity);
            return Ok(token);
        }
        
        return Ok(account);
    }
}