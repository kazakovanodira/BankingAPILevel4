using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Services;

public class AccountsServices(IAccountRepository _accountRepository, ICurrencyServices _currencyServices) : IAccountsService
{
    public ApiResponse<AccountResponse> CreateAccount(CreateAccountRequest request)
    {
        var newAccount = new AccountResponse(Guid.NewGuid(), request.Name, 0);
        
        return new ApiResponse<AccountResponse>
        {
            Result = _accountRepository.AddAccount(newAccount).Result,
            HttpStatusCode = 201
        };
    }

    public ApiResponse<AccountResponse> GetAccount(AccountRequest request)
    {
        var account = _accountRepository.GetAccountById(request);
        
        if (account is null)
        {
            return new ApiResponse<AccountResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        return new ApiResponse<AccountResponse>
        {
            Result = account.Result,
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeDeposit(TransactionRequest request)
    {
        var account = _accountRepository.GetAccountById(
            new AccountRequest
            {
                AccountId = request.SenderAccId
            });

        if (account is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        var newBalance = account.Result.Balance + request.Amount;
        _accountRepository.UpdateAccount(new AccountResponse(request.SenderAccId, account.Result.Name, newBalance));

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Result.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeWithdraw(TransactionRequest request)
    {
        var account = _accountRepository.GetAccountById(
            new AccountRequest
            {
                AccountId = request.SenderAccId
            });

        if (account is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        if (account.Result.Balance < request.Amount)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Not enough balance.",
                HttpStatusCode = 400
            };
        }
        
        var newBalance = account.Result.Balance - request.Amount;
        _accountRepository.UpdateAccount(new AccountResponse(request.SenderAccId, account.Result.Name, newBalance));

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Result.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeTransfer(TransactionRequest request)
    {
        var sender = _accountRepository.GetAccountById(
            new AccountRequest
            {
                AccountId = request.SenderAccId
            });
        
        var receiver = _accountRepository.GetAccountById(
            new AccountRequest
            {
                AccountId = request.ReceiverAccId ?? Guid.Empty
            });

        if (sender is null || receiver is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        if (sender.Result.Balance < request.Amount)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Not enough balance.",
                HttpStatusCode = 400
            };
        }
        
        var newSenderBalance = sender.Result.Balance - request.Amount;
        var newReceiverBalance = sender.Result.Balance + request.Amount;

        _accountRepository.UpdateAccount(new AccountResponse(request.SenderAccId, sender.Result.Name, newSenderBalance));
        _accountRepository.UpdateAccount(new AccountResponse(request.ReceiverAccId ?? Guid.Empty, receiver.Result.Name, newReceiverBalance));

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(sender.Result.Balance),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<ConvertedBalances>> GetConvertedBalanceAsync(AccountRequest accountRequest, CurrencyRequest currencyRequest)
    {
        var account = _accountRepository.GetAccountById(accountRequest);
        if (account is null)
        {
            return new ApiResponse<ConvertedBalances>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }

        string[] requestedCurrencies = currencyRequest.Currency?.Split(
                                           ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                            ?? Array.Empty<string>();
        Console.WriteLine("Requested currencies: " + string.Join(", ", requestedCurrencies));

        var fetchedCurrencies = await _currencyServices.ConvertToCurrency();

        var convertedBalances = new ConvertedBalances();

        foreach (var currency in requestedCurrencies)
        {
            if (fetchedCurrencies.ContainsKey(currency))
            {
                convertedBalances.convertedBalances[currency] = fetchedCurrencies[currency] * account.Result.Balance;
            }
        }

        return new ApiResponse<ConvertedBalances>
        {
            Result = convertedBalances,
            HttpStatusCode = 200
        };
    }
}