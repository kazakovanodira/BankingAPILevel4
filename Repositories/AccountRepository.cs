using BankingAPILevel4.Data;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models;
using BankingAPILevel4.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPILevel4.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
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

    public async Task<User?> GetAccountByAccountNumber(Guid? accountNumber) =>
        await _context.Users.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
    
    public async Task<User?> GetAccountById(int id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    
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