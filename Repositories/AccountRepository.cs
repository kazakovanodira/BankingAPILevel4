using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Responses;
using banking_api_repo.Mapper;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsContext _context;

    public AccountRepository(AccountsContext context)
    {
        _context = context;
    }
    
    public async Task<AccountDto> AddAccount(AccountDto accountDto)
    {
        var account = new Account
        {
            AccountNumber = accountDto.AccountId,
            Balance = accountDto.Balance,
            Name = accountDto.Name
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return accountDto;
    }

    public async Task<AccountDto?> UpdateAccount(Guid accountId, decimal balance)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == accountId);
        
        if (account is null)
        {
            return null;
        }
        
        account.Balance = balance;
        
        await _context.SaveChangesAsync();
        
        return ManualMapper.ConvertToDto(account);
    }

    public async Task<AccountDto?> GetAccountById(Guid accountId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(account => 
            account.AccountNumber == accountId);
        
        if (account is null)
        {
            return null;
        }
        
        return ManualMapper.ConvertToDto(account);
    }
}