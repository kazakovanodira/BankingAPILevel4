using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace banking_api_repo.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountsService _service;
    private readonly IConfiguration _configuration;
    private readonly UserContext _ctx;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAuthenticationServices _authenticationServices;
    
    public AuthenticationController(IAccountsService service, IConfiguration configuration,
        UserContext ctx, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, IAuthenticationServices authenticationServices,
        SignInManager<User> signInManager)
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
    public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
    {
        var account = await _service.CreateAccount(request);
        var identity = new User { UserName = request.Username };
        var newClaims = new List<Claim>
        {
            new("Name", request.Name)
        };

        await _userManager.AddClaimsAsync(identity, newClaims);

        if (request.Role is Role.Administrator)
        {
            var role = await _roleManager.FindByNameAsync("Administrator");
            if (role is null)
            {
                role = new IdentityRole("Administrator");
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(identity, "Administrator");
            
            newClaims.Add(new Claim(ClaimTypes.Role, "Administrator"));
        }
        else
        {
            var role = await _roleManager.FindByNameAsync("User");
            if (role is null)
            {
                role = new IdentityRole("User");
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(identity, "User");
            
            newClaims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, identity.UserName ?? throw new InvalidOperationException()),
            new(JwtRegisteredClaimNames.Email, identity.UserName ?? throw new InvalidOperationException())
        });
        
        claimsIdentity.AddClaims(newClaims);

        var token = _authenticationServices.CreateSecurityToken(claimsIdentity);
        //var response = new AuthenticationResult(_authenticationServices.WriteToken(token));
        return Ok(token);

        /*if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }

        return CreatedAtAction(nameof(GetAccount),
            new { accountNumber = account.Result.Id }, account);*/
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginUser login)
    {
        var account = await _userManager.FindByNameAsync(login.Username);
        if (account is null) return BadRequest();

        var result = await _signInManager.CheckPasswordSignInAsync(account, login.Password, false);
        if (!result.Succeeded) return BadRequest("Couldn't sign in.");

        var claims = await _userManager.GetClaimsAsync(account);

        var roles = await _userManager.GetRolesAsync(account);
        
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, account.UserName ?? throw new InvalidOperationException()),
            new(JwtRegisteredClaimNames.Email, account.UserName ?? throw new InvalidOperationException())
        });
        
        claimsIdentity.AddClaims(claims);

        foreach (var role in roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        var token = _authenticationServices.CreateSecurityToken(claimsIdentity);
        //var response = new AuthenticationResult(_authenticationServices.WriteToken(token));
        return Ok(token);
    }

    public class LoginUser
    {
        public string Username;
        public string Password;
    }
}