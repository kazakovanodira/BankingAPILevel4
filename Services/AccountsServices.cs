using AutoMapper;
using banking_api_repo.Hasher;
using banking_api_repo.Interfaces;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

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
        request.Password = Md5Hasher.ComputeHash(request.Password);
        
        var user = _mapper.Map<User>(request);

        if (await _accountRepository.GetAccountByUserName(user.Username) != null)
        {
            return new ApiResponse<AccountDto>
            {
                ErrorMessage = "Account with this username already exists.",
                HttpStatusCode = 409
            };
        }
        
        return new ApiResponse<AccountDto>
        {
            Result = _mapper.Map<AccountDto>(await _accountRepository.AddAccount(user)),
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
    
    public async Task<ApiResponse<LoginResponse>> CheckIfPasswordsMatchesUsername(LoginRequest loginDetails)
    {
        loginDetails.Password = Md5Hasher.ComputeHash(loginDetails.Password);

        var account = await _accountRepository.GetAccountByUserName(loginDetails.Username);
        
        if (account is null)
        {
            return new ApiResponse<LoginResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }

        if (account.Password != loginDetails.Password)
        {
            return new ApiResponse<LoginResponse>
            {
                ErrorMessage = "Password doesn't match the username.",
                HttpStatusCode = 401
            };
        }
        
        return new ApiResponse<LoginResponse>
        {
            Result = _mapper.Map<LoginResponse>(account),
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