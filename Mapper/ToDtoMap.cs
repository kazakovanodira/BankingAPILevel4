using banking_api_repo.Models;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Mapper;

public static class ManualMapper
{
    public static AccountDto ConvertToDto(Account account)
    {
        return new AccountDto(account.AccountNumber, account.Name, account.Balance);
    }
}