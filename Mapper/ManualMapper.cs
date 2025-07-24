using banking_api_repo.Models;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Mapper;

public static class ManualMapper
{
    public static AccountDto ConvertToDto(Account account) => new AccountDto(account.AccountNumber, account.Name, account.Balance);

    public static IEnumerable<AccountDto> ConvertToEnumerableDto(IEnumerable<Account> accounts)
    {
        return accounts.Select(account => ConvertToDto(account));
    }
}