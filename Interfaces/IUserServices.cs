using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;

namespace BankingAPILevel4.Interfaces;

public interface IUserServices
{
    Task<ApiResponse<AccountDto>> CreateAccount(CreateAccountRequest createAccountRequest);
}