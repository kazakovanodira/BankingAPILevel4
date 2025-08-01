using banking_api_repo.Models;
using banking_api_repo.Services;

namespace banking_api_repo.Interfaces;

public interface IAccountRepository
{
    Task<User> AddAccount(User user);
    Task<User?> UpdateAccount(User user, decimal amount);
    Task<User?> GetAccountByAccountNumber(Guid? accountNumber);
    Task<User?> GetAccountByUserName(string username);
    Task<IEnumerable<User>> GetAccountsAsync();
    Task<(IEnumerable<User>, PaginationMetadata)> GetAccountsAsync(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending);
}