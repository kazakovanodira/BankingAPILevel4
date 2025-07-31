using banking_api_repo.Data;
using banking_api_repo.Interfaces;
using banking_api_repo.Models;
using banking_api_repo.Services;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserContext _context;

    public AccountRepository(UserContext context)
    {
        _context = context;
    }
    
    public async Task<User> AddAccount(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAccount(User user, decimal amount)
    {
        user.Balance += amount;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetAccountByAccountNumber(Guid accountNumber) => 
        await _context.Users.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
    
    public async Task<User?> GetAccountByUserName(string username) => 
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    
    public async Task<IEnumerable<User>> GetAccountsAsync() =>
        await _context.Users.OrderBy(u => u.Name).ToListAsync();
    

    public async Task<(IEnumerable<User>, PaginationMetadata)> GetAccountsAsync(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending)
    {
        var accountsCollection = _context.Users as IQueryable<User>;

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            accountsCollection = accountsCollection.Where(u => u.Name == name);
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