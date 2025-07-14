using banking_api_repo.Data;
using banking_api_repo.Interface;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace RESTAPIBankingApplication.Services;

public class AccountsServices : IAccountsService
{
    private readonly AccountsContext _context;
    
    public AccountsServices(AccountsContext context)
    {
        _context = context;
    }

    public ApiResponse<AccountResponse> CreateAccount(CreateAccountRequest request)
    {
        var account = new Account
        {
            Name = request.Name,
            Balance = 0,
            AccountNumber = Guid.NewGuid(),
        };
        
        _context.Accounts.Add(account);
        _context.SaveChanges();

        return new ApiResponse<AccountResponse>
        {
            Result = new AccountResponse(account.AccountNumber, account.Name, account.Balance),
            HttpStatusCode = 201
        };
    }

    public ApiResponse<AccountResponse> GetAccount(AccountRequest request)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == request.AccountId);
        
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
            Result = new AccountResponse(account.AccountNumber, account.Name, account.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeDeposit(TransactionRequest request)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == request.SenderAccId);
        
        if (account is null)
        {
            return new ApiResponse<BalanceResponse>
            {
                ErrorMessage = "Account not found.",
                HttpStatusCode = 404
            };
        }
        
        account.Balance += request.Amount;
        _context.SaveChanges();
        
        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeWithdraw(TransactionRequest request)
    {
        var account = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == request.SenderAccId);
        
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
        
        account.Balance -= request.Amount;
        _context.SaveChanges();
        
        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(account.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<BalanceResponse> MakeTransfer(TransactionRequest request)
    {
        var sender = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == request.SenderAccId);
        var receiver = _context.Accounts.FirstOrDefault(account => 
            account.AccountNumber == request.ReceiverAccId);
        
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
        
        sender.Balance -= request.Amount;
        receiver.Balance += request.Amount;
        _context.SaveChanges();
        
        return new ApiResponse<BalanceResponse>
        {
            Result = new BalanceResponse(sender.Balance),
            HttpStatusCode = 200
        };
    }

    public ApiResponse<ConvertedBalances> CheckBalance(AccountRequest accountRequest, CurrencyRequest currencyRequest)
    {
        
    }
}