using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models.Responses;
using banking_api_repo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountsService _service;
    private readonly IConfiguration _configuration;
    private readonly UserContext _ctx;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AuthenticationServices _authenticationServices;
    
    public AuthenticationController(IAccountsService service, IConfiguration configuration,
        UserContext ctx, UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, AuthenticationServices authenticationServices,
        SignInManager<IdentityUser> signInManager)
    {
        _service = service;
        _configuration = configuration ?? 
                         throw new ArgumentNullException(nameof(configuration));
        _ctx = ctx;
        _userManager = userManager;
        _roleManager = roleManager;
        _authenticationServices = authenticationServices;
        _signInManager = signInManager;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="authenticationRequestBody"></param>
    /// <returns></returns>
    [HttpPost("register")]
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
        claimsForToken.Add(new Claim("sub", user.Id.ToString()));
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
    
    public class AuthenticationRequestBody
    {
        [Required(ErrorMessage = "Username is required to authenticate")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required to authenticate.")]
        public string Password { get; set; }

        public Role Role;
    }

    public enum Role
    {
        Administrator,
        User
    }
    
    private AccountDto? ValidateUserCredentials(string userName, string password)
    {
        var response = _service.FindAccountByUsername(userName);
        if (response.Result.IsSuccess)
        {
            return response.Result.Result;
        }
        
        return null;
    }
}