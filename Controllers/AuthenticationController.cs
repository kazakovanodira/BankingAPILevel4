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
    private readonly IAccountsService _service;
    private readonly IAuthenticationServices _authenticationServices;
    
    public AuthenticationController(IAccountsService service, 
        IAuthenticationServices authenticationServices)
    {
        _service = service;
        _authenticationServices = authenticationServices;
    }
    
    /// <summary>
    /// Registers a new user account and returns a JWT token upon successful registration.
    /// </summary>
    /// <param name="request">The request payload containing user registration details such as name, username, password, and role.</param>
    /// <returns>
    /// Returns ApiResponse containing a JWT token if the account is created successfully.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
    {
        var account = await _service.CreateAccount(request);
        
        var newClaims = new List<Claim>
        {
            new("Name", request.Name)
        };
        
        if (request.Role is "Admin")
        {
            newClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            newClaims.Add(new Claim(ClaimTypes.Role, "User"));
        }
        
        var claimsIdentity = new ClaimsIdentity(newClaims, "Bearer");


        var token = _authenticationServices.CreateSecurityToken(claimsIdentity);
        return Ok(token);
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
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var account = await _service.CheckIfPasswordsMatchesUsername(login);

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