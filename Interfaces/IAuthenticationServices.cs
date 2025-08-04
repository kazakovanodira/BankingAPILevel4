using System.Security.Claims;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interfaces;

public interface IAuthenticationServices
{
    string CreateSecurityToken(ClaimsIdentity identity);
    Task<ApiResponse<LoginResponse>> CheckIfPasswordsMatchesUsername(LoginRequest loginDetails);
}