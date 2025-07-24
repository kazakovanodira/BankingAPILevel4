using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountsService _service;
    private readonly IConfiguration _configuration;
    
    public AuthenticationController(IAccountsService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration ?? 
                         throw new ArgumentNullException(nameof(configuration));
    }
    
    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    private record bankUser(
        string UserName,
        string Name,
        Guid AccountNumber,
        decimal Balance);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="authenticationRequestBody"></param>
    /// <returns></returns>
    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
    {
        var user = ValidateUserCredentials(
            authenticationRequestBody.UserName, 
            authenticationRequestBody.Password);

        if (user is null)
        {
            return Unauthorized();
        }

        var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", user.AccountNumber.ToString()));
        claimsForToken.Add(new Claim("name", user.Name));
        claimsForToken.Add(new Claim("balance", user.Balance.ToString("C")));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }

    private bankUser ValidateUserCredentials(string? userName, string? password)
    {
        return new bankUser("nodirak", "nodira", Guid.Empty, 0);
    }
}