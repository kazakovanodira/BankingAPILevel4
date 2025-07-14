using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace banking_api_repo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountsService service) : ControllerBase
{
    /// <summary>
    /// Creates a new bank account with the specified account holder name.
    /// </summary>
    /// <param name="request">Contains the account holder's name.</param>
    /// <returns>The created account details or an error response.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status400BadRequest)]
    public IActionResult  CreateNewAccount([FromBody] CreateAccountRequest request)
    {
        var account = service.CreateAccount(request);
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        
        return CreatedAtAction(nameof(GetAccount), new { accountNumber = account.Result.AccountId }, account);
    }
    
    /// <summary>
    /// Retrieves account details by account number.
    /// </summary>
    /// <param name="accountNumber">The unique identifier of the account.</param>
    /// <returns>The account details or an error response if not found.</returns>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AccountResponse>), StatusCodes.Status404NotFound)]
    public IActionResult  GetAccount(Guid accountNumber)
    {
        var account = service.GetAccount(new AccountRequest { AccountId = accountNumber });
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Makes a deposit into the specified account.
    /// </summary>
    /// <param name="accountNumber">The account number to deposit into.</param>
    /// <param name="request">The deposit request containing the deposit amount.</param>
    /// <returns>The updated balance or an error response.</returns>
    [HttpPost("{accountNumber}/deposits")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public IActionResult  MakeDeposit(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = service.MakeDeposit(new TransactionRequest()
        {
            SenderAccId = accountNumber,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Makes a withdrawal from the specified account.
    /// </summary>
    /// <param name="accountNumber">The account number to withdraw from.</param>
    /// <param name="request">The withdrawal request containing the withdrawal amount.</param>
    /// <returns>The updated balance or an error response.</returns>
    [HttpPost("{accountNumber}/withdrawals")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public IActionResult  MakeWithdrawal(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = service.MakeWithdraw(new TransactionRequest()
        {
            SenderAccId = accountNumber,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Transfers funds between two accounts.
    /// </summary>
    /// <param name="request">The transfer request containing sender, receiver, and transfer amount.</param>
    /// <returns>The updated sender balance or an error response.</returns>
    [HttpPost("transfers")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public IActionResult  MakeTransfer([FromBody] TransactionRequest request)
    {
        var account = service.MakeTransfer(new TransactionRequest()
        {
            SenderAccId = request.SenderAccId,
            ReceiverAccId = request.ReceiverAccId,
            Amount = request.Amount,
        });
        
        if (!account.IsSuccess)
        {
            return StatusCode(account.HttpStatusCode, account);
        }
        return Ok(account);
    }
    
    /// <summary>
    /// Converts balance of the specified account to the requested currencies.
    /// </summary>
    /// <param name="accountNumber">The account number to check balance from</param>
    /// <param name="currencyRequest">The currencies to convert the balance to.</param>
    /// <returns>The list of the converted balance or an error response</returns>
    [HttpGet("{accountNumber}/balances")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    public IActionResult CheckConvertedBalance(Guid accountNumber, [FromBody] CurrencyRequest currencyRequest)
    {
        var response = service.GetConvertedBalance(new AccountRequest { AccountId = accountNumber }, 
            new CurrencyRequest {  Currency = currencyRequest.Currency });

        if (!response.IsSuccess)
        {
            return StatusCode(response.HttpStatusCode, response);
        }
        return Ok(response);
    }
}