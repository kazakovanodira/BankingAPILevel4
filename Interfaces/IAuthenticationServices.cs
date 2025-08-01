using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace banking_api_repo.Interfaces;

public interface IAuthenticationServices
{
    string CreateSecurityToken(ClaimsIdentity identity);
}