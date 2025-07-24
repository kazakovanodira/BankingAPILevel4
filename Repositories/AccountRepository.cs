using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsContext _context;

    public AccountRepository(AccountsContext context)
    {
        _context = context;
    }
    
    public async Task<Account> AddAccount(AccountDto accountDto)
    {
        var account = new Account
        {
            AccountNumber = accountDto.AccountId,
            Balance = accountDto.Balance,
            Name = accountDto.Name
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> UpdateAccount(Account account, decimal amount)
    {
        account.Balance += amount;
        
        await _context.SaveChangesAsync();
        
        return account;
    }

    public async Task<Account?> GetAccountById(Guid accountId) =>
        await _context.Accounts.FirstOrDefaultAsync(account => account.AccountNumber == accountId);
    
    public async Task<IEnumerable<Account>> GetAccountsAsync() =>
        await _context.Accounts.OrderBy(account => account.Name).ToListAsync();
    

    public async Task<IEnumerable<Account>> GetAccountsAsync(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return await GetAccountsAsync();
        }

        name = name.Trim();
        return await _context.Accounts
            .Where(account => account.Name == name)
            .OrderBy(account => account.Name)
            .ToListAsync();
    }
}