using System.Text;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using Microsoft.Extensions.Options;

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
}