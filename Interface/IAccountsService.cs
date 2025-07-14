using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Interface;

public interface IAccountsService
{
    ApiResponse<AccountResponse> CreateAccount(CreateAccountRequest request);
    ApiResponse<AccountResponse> GetAccount(AccountRequest request);
    ApiResponse<BalanceResponse> MakeDeposit(TransactionRequest request);
    ApiResponse<BalanceResponse> MakeWithdraw(TransactionRequest request);
    ApiResponse<BalanceResponse> MakeTransfer(TransactionRequest request);
    ApiResponse<ConvertedBalances> CheckBalance(AccountRequest accountRequest, CurrencyRequest currencyRequest);
}