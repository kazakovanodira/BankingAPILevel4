using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAccountRepository
{
    public Task<AccountResponse> AddAccount(AccountResponse accountResponse);
    public Task<AccountResponse>? UpdateAccount(AccountResponse accountResponse);
    public Task<AccountResponse>? GetAccountById(AccountRequest accountRequest);
}