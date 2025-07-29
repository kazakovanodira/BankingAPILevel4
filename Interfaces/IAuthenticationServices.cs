using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAuthenticationServices
{
    Task<ApiResponse<AccountDto?>> ValidateUserCredentials(string userName, string password);
}