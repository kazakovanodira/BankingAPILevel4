using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAccountRepository
{
    Task<Account> AddAccount(AccountDto accountDto);
    Task<Account?> UpdateAccount(Account account, decimal amount);
    Task<Account?> GetAccountById(Guid accountId);
}