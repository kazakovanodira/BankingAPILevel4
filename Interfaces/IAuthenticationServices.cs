using System.Security.Claims;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;

namespace BankingAPILevel4.Interfaces;

public interface IAuthenticationServices
{
    string CreateSecurityToken(ClaimsIdentity identity);
    Task<ApiResponse<LoginResponse>> CheckIfPasswordsMatchesUsername(LoginRequest loginDetails);
}