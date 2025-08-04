using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using banking_api_repo.Services;

namespace banking_api_repo.Interfaces;

public interface IAccountsService
{
    Task<ApiResponse<AccountDto>> CreateAccount(CreateAccountRequest request);
    Task<ApiResponse<AccountDto>> GetAccount(AccountRequest request);
    Task<ApiResponse<(IEnumerable<AccountDto>, PaginationMetadata)>>  GetAccounts(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending);
    Task<ApiResponse<BalanceResponse>> MakeDeposit(TransactionRequest request);
    Task<ApiResponse<BalanceResponse>> MakeWithdraw(TransactionRequest request);
    Task<ApiResponse<BalanceResponse>> MakeTransfer(TransactionRequest request);
    Task<ApiResponse<ConvertedBalances>> GetConvertedBalanceAsync(
        AccountRequest accountRequest, CurrencyRequest currencyRequest);
}