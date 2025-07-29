using AutoMapper;
using banking_api_repo.Interface;
using banking_api_repo.Mapper;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace banking_api_repo.Services;

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
    
    public async Task<ApiResponse<AccountDto>> CreateAccount(CreateAccountRequest request)
    {
        var user = _mapper.Map<User>(request);
        var (result, newUser) = await _accountRepository.AddAccount(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            return new ApiResponse<AccountDto>
            {
                ErrorMessage = errorMessage,
                HttpStatusCode = 409
            };
        }
        return new ApiResponse<AccountDto>
        {
            Result = _mapper.Map<AccountDto>(newUser),
            HttpStatusCode = 201
        };
    }
    
    public async Task<ApiResponse<(IEnumerable<AccountDto>, PaginationMetadata)>> GetAccounts(string? name, 
        int pageNumber, 
        int pageSize,
        string? orderBy,
        bool descending)
    {
        var (accounts, paginationMetadata) = await _accountRepository.GetAccountsAsync(name, pageNumber, pageSize, orderBy, descending);

        var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
        
        return new ApiResponse<(IEnumerable<AccountDto>, PaginationMetadata)>
        {
            Result = (accountDtos, paginationMetadata),
            HttpStatusCode = 200
        };
    }

    public async Task<ApiResponse<AccountDto>> GetAccount(AccountRequest request)
    {
        var account = await _accountRepository.GetAccountById(request.AccountId);
        
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
    
    public async Task<ApiResponse<AccountDto>> FindAccountByUsername(string username)
    {
        var account = await _accountRepository.GetAccountByUserName(username);
        
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
        var account = await _accountRepository.GetAccountById(request.SenderAccId);

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
        var account = await _accountRepository.GetAccountById(request.SenderAccId);

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
        var sender = await _accountRepository.GetAccountById(request.SenderAccId);
        
        var receiver = await _accountRepository.GetAccountById(request.ReceiverAccId);

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
        var account = await _accountRepository.GetAccountById(accountRequest.AccountId);
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