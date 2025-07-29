using banking_api_repo.Models;
using banking_api_repo.Services;
using Microsoft.AspNetCore.Identity;

namespace banking_api_repo.Interface;

public interface IAccountRepository
{
    Task<(IdentityResult, User)> AddAccount(User user, string password);
    Task<(IdentityResult, User)> UpdateAccount(User user, decimal amount);
    Task<User?> GetAccountById(string accountId);
    Task<User?> GetAccountByUserName(string username);
    Task<IEnumerable<User>> GetAccountsAsync();
    Task<(IEnumerable<User>, PaginationMetadata)> GetAccountsAsync(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending);
}