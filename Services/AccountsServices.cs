using AutoMapper;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;

namespace BankingAPILevel4.Services;

public class AccountsServices : IAccountsService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyServices _currencyServices;
    private readonly IMapper _mapper;

    public AccountsServices(
        IAccountRepository accountRepository, 
        ICurrencyServices currencyServices, 
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _currencyServices = currencyServices;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<(IEnumerable<AccountDto>, PaginationMetadata)>> GetAccounts(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending)
    {
        var (accounts, paginationMetadata) = await _accountRepository.GetAccountsAsync(
            name, 
            pageNumber, 
            pageSize, 
            orderBy, 
            descending);

        var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
        
        return new ApiResponse<(IEnumerable<AccountDto>, PaginationMetadata)>
        {
            Result = (accountDtos, paginationMetadata),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<AccountDto>> GetAccount(AccountRequest request)
    {
        var account = await _accountRepository.GetAccountByAccountNumber(request.AccountNumber);
        
        if (account is null)
        {
            return new ApiResponse<AccountDto>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        return new ApiResponse<AccountDto>
        {
            Result = _mapper.Map<AccountDto>(account),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<BalanceResponse>> MakeDeposit(TransactionRequest request)
    {
        var account = await _accountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);

        if (account is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        await _accountRepository.UpdateAccount(account, request.Amount);

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Balance),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<BalanceResponse>> MakeWithdraw(TransactionRequest request)
    {
        var account = await _accountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);

        if (account is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        if (account.Balance < request.Amount)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Not enough balance.",
                HttpStatusCode = 400
            };
        }
        
        await _accountRepository.UpdateAccount(account, request.Amount*-1);

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Balance),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<BalanceResponse>> MakeTransfer(TransactionRequest request)
    {
        var sender = await _accountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);
        
        var receiver = await _accountRepository.GetAccountByAccountNumber(request.ReceiverAccountNumber);

        if (sender is null || receiver is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        if (sender.Balance < request.Amount)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Not enough balance.",
                HttpStatusCode = 400
            };
        }
        
        await _accountRepository.UpdateAccount(sender, request.Amount*-1);
        await _accountRepository.UpdateAccount(receiver, request.Amount);

        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(sender.Balance),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<ConvertedBalances>> GetConvertedBalanceAsync(AccountRequest accountRequest, CurrencyRequest currencyRequest)
    {
        var account = await _accountRepository.GetAccountByAccountNumber(accountRequest.AccountNumber);
        
        if (account is null)
        {
            return new ApiResponse<ConvertedBalances>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        var fetchedCurrencies = await _currencyServices.FetchExchangeRates(currencyRequest);

        var convertedBalances = new ConvertedBalances();

        if (fetchedCurrencies is null)
        {
            return new ApiResponse<ConvertedBalances>
            {
                ErrorMessage = "API didn't give any response.",
                HttpStatusCode = 404
            };
        }

        foreach (var currency in fetchedCurrencies)
        {
            convertedBalances.Add(currency.Key, Math.Round(fetchedCurrencies[currency.Key] * account.Balance, 2));
        }
        
        return new ApiResponse<ConvertedBalances>
        {
            Result = convertedBalances,
            HttpStatusCode = 200
        };
    }
}