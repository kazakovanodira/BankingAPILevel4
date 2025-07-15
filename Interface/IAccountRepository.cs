using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAccountRepository
{
    Task<AccountDto> AddAccount(AccountDto accountDto);
    Task<AccountDto?> UpdateAccount(Guid accountId, decimal balance);
    Task<AccountDto?> GetAccountById(Guid accountId);
}