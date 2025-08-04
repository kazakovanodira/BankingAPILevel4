using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;
using BankingAPILevel4.Services;

namespace BankingAPILevel4.Interfaces;

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