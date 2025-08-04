using BankingAPILevel4.Models;
using BankingAPILevel4.Services;

namespace BankingAPILevel4.Interfaces;

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