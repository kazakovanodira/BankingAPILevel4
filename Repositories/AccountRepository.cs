using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Responses;
using banking_api_repo.Services;
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
    

    public async Task<(IEnumerable<Account>, PaginationMetadata)> GetAccountsAsync(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending)
    {
        var accountsCollection = _context.Accounts as IQueryable<Account>;

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            accountsCollection = accountsCollection.Where(account => account.Name == name);
        }
        
        if (!string.IsNullOrEmpty(orderBy))
        {
            accountsCollection = orderBy.ToLower() switch
            {
                "balance" => descending
                    ? accountsCollection.OrderByDescending(a => a.Balance)
                    : accountsCollection.OrderBy(a => a.Balance),

                _ => descending
                    ? accountsCollection.OrderByDescending(a => a.Name)
                    : accountsCollection.OrderBy(a => a.Name),
            };
        }

        var totalItemCount = await accountsCollection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await accountsCollection
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();
        
        return (collectionToReturn, paginationMetadata);
    }
}