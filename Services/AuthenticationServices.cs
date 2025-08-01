using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using banking_api_repo.Interfaces;
using banking_api_repo.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly JwtSettings? _settings;
    private readonly byte[] _key;

    public AuthenticationServices(IOptions<JwtSettings> jwtOptions)
    {
        _settings = jwtOptions.Value;
        ArgumentNullException.ThrowIfNull(_settings);
        ArgumentNullException.ThrowIfNull(_settings.SigningKey);
        ArgumentNullException.ThrowIfNull(_settings.Audiences);
        ArgumentNullException.ThrowIfNull(_settings.Audiences[0]);
        ArgumentNullException.ThrowIfNull(_settings.Issuer);
        _key = Encoding.ASCII.GetBytes(_settings?.SigningKey!);
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
}