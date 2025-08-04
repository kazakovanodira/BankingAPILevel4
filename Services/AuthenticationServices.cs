using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using banking_api_repo.Hasher;
using banking_api_repo.Interfaces;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;
    private readonly JwtSettings? _settings;
    private readonly byte[] _key;

    public AuthenticationServices(IOptions<JwtSettings> jwtOptions, 
            IAccountRepository accountRepository, 
            IMapper mapper)
    {
        _settings = jwtOptions.Value;
        ArgumentNullException.ThrowIfNull(_settings);
        ArgumentNullException.ThrowIfNull(_settings.SigningKey);
        ArgumentNullException.ThrowIfNull(_settings.Audiences);
        ArgumentNullException.ThrowIfNull(_settings.Audiences[0]);
        ArgumentNullException.ThrowIfNull(_settings.Issuer);
        _key = Encoding.ASCII.GetBytes(_settings?.SigningKey!);
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    private static JwtSecurityTokenHandler TokenHandler => new();

    public string CreateSecurityToken(ClaimsIdentity identity)
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
    
    public async Task<ApiResponse<LoginResponse>> CheckIfPasswordsMatchesUsername(LoginRequest loginDetails)
    {
        loginDetails.Password = Md5Hasher.ComputeHash(loginDetails.Password);

        var account = await _accountRepository.GetAccountByUserName(loginDetails.Username);
        
        if (account is null)
        {
            return new ApiResponse<LoginResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }

        if (account.Password != loginDetails.Password)
        {
            return new ApiResponse<LoginResponse>
            {
                ErrorMessage = "Password doesn't match the username.",
                HttpStatusCode = 401
            };
        }
        
        return new ApiResponse<LoginResponse>
        {
            Result = _mapper.Map<LoginResponse>(account),
            HttpStatusCode = 200
        };
    }
}