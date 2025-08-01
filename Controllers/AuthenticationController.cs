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
    /// 
    /// </summary>
    /// <param name="authenticationRequestBody"></param>
    /// <returns></returns>
    [HttpPost("register")]
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

    [HttpPost("login")]
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