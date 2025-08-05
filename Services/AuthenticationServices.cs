using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BankingAPILevel4.Hasher;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankingAPILevel4.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly JwtSettings? _settings;
    private readonly byte[] _key;

    public AuthenticationServices(IOptions<JwtSettings> jwtOptions, 
        IAuthenticationRepository authenticationRepository, 
        IAccountRepository accountRepository)
    {
        _settings = jwtOptions.Value;
        ArgumentNullException.ThrowIfNull(_settings);
        ArgumentNullException.ThrowIfNull(_settings.SigningKey);
        ArgumentNullException.ThrowIfNull(_settings.Audiences);
        ArgumentNullException.ThrowIfNull(_settings.Audiences[0]);
        ArgumentNullException.ThrowIfNull(_settings.Issuer);
        _key = Encoding.ASCII.GetBytes(_settings?.SigningKey!);
        _authenticationRepository = authenticationRepository;
        _accountRepository = accountRepository;
    }
    
    public async Task<ApiResponse<string>> GetToken(LoginRequest loginDetails)
    {
        loginDetails.Password = Md5Hasher.ComputeHash(loginDetails.Password);

        var accountCredentials = await _authenticationRepository.Get(loginDetails.Username);
        
        if (accountCredentials is null)
        {
            return new ApiResponse<string>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }

        if (accountCredentials.Password != loginDetails.Password)
        {
            return new ApiResponse<string>
            {
                ErrorMessage = "Password doesn't match the username.",
                HttpStatusCode = 401
            };
        }
        
        var accountDetails = await _accountRepository.GetAccountById(accountCredentials.UserId);

        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, accountCredentials.Username),
            new(ClaimTypes.Name, accountDetails.Name),
            new(ClaimTypes.Role, accountDetails.Role)
        }, "Bearer");

        var token = CreateSecurityToken(claimsIdentity);
        
        return new ApiResponse<string>
        {
            Result = token,
            HttpStatusCode = 200
        };
    }
    
    private static JwtSecurityTokenHandler TokenHandler => new();

    private string CreateSecurityToken(ClaimsIdentity identity)
    {
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = identity,
            Expires = DateTime.Now.AddHours(5),
            Audience = _settings!.Audiences?[0]!,
            Issuer = _settings.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return TokenHandler.WriteToken(token);
    }
}