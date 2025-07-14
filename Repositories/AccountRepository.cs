using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Repositories;

public class AccountRepository(AccountsContext _context) : IAccountRepository
{
    public Task<AccountResponse> AddAccount(AccountResponse accountResponse)
    {
        var account = new Account
        {
            AccountNumber = accountResponse.AccountId,
            Balance = accountResponse.Balance,
            Name = accountResponse.Name

        };
        
        _context.Accounts.Add(account);
        _context.SaveChanges();

        return Task.FromResult(new AccountResponse(account.AccountNumber, account.Name, account.Balance));
    }

    public Task<AccountResponse>? UpdateAccount(AccountResponse accountResponse)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == accountResponse.AccountId);
        
        if (account is null)
        {
            return null;
        }
        
        account.AccountNumber = accountResponse.AccountId;
        account.Balance = accountResponse.Balance;
        account.Name = accountResponse.Name;
        
        _context.SaveChanges();

        return Task.FromResult(new AccountResponse(account.AccountNumber, account.Name, account.Balance));
    }

    public Task<AccountResponse>? GetAccountById(AccountRequest accountRequest)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == accountRequest.AccountId);
        
        if (account is null)
        {
            return null;
        }

        return Task.FromResult(new AccountResponse(account.AccountNumber, account.Name, account.Balance));
    }
}