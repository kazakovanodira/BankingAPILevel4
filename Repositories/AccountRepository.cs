using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserContext _context;
    private readonly UserManager<User> _userManager;

    public AccountRepository(UserContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<(IdentityResult, User)> AddAccount(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        await _context.SaveChangesAsync();
        return (result, user);
    }

    public async Task<(IdentityResult, User)> UpdateAccount(User user, decimal amount)
    {
        user.Balance += amount;
        var result = await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        return (result, user);
    }

    public async Task<User?> GetAccountById(Guid accountId) => 
        await _userManager.Users.FirstOrDefaultAsync(u => u.AccountNumber == accountId);
    
    public async Task<User?> GetAccountByUserName(string username) => 
        await _userManager.FindByNameAsync(username);
    
    public async Task<IEnumerable<User>> GetAccountsAsync() =>
        await _userManager.Users.OrderBy(u => u.Name).ToListAsync();
    

    public async Task<(IEnumerable<User>, PaginationMetadata)> GetAccountsAsync(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending)
    {
        var accountsCollection = _userManager.Users;

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