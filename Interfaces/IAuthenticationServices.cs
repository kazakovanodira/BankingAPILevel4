using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Interface;

public interface IAuthenticationServices
{
    SecurityToken CreateSecurityToken(ClaimsIdentity identity);
    string WriteToken(SecurityToken token);
}