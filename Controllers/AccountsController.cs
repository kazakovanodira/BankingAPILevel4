using System.Text.Json;
using Asp.Versioning;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPILevel4.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/accounts")]
[ApiVersion(1)]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _service;
    private const int MaxAccountsPageSize = 10;

    public AccountsController(IAccountsService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Registers a new user account and returns the new account details.
    /// </summary>
    /// <param name="request">The request payload containing user registration details such as name, username, password, and role.</param>
    /// <returns>
    /// Returns new account details.
    /// </returns>
    [HttpPost("NewAccount")]
    [AllowAnonymous] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _service.CreateAccount(request);
        
        return Ok(account.Result);
    }
    
    /// <summary>
    /// Retrieves accounts with the specified name or the list of all accounts otherwise.
    /// </summary>
    /// <param name="name">The name to retrieve accounts by.</param>
    /// <param name="pageNumber">The number of the page that the client requested.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>The list of accounts with the specified name or the list of all accounts.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    [Produces("application/json", "text/csv")]
    public async Task<IActionResult> GetAccounts([FromQuery]string? name, 
        int pageNumber = 1, 
        int pageSize = 5,
        string? orderBy = "Name",
        bool descending = false)
    {
        if (pageSize > MaxAccountsPageSize)
        {
            pageSize = MaxAccountsPageSize;
        }
        
        var response = await _service.GetAccounts(name, pageNumber, pageSize, orderBy, descending);
        
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.Result.Item2));
        
        return Ok(response.Result.Item1);
    } 
    
    /// <summary>
    /// Retrieves account details by account number.
    /// </summary>
    /// <param name="accountNumber">The unique identifier of the account.</param>
    /// <returns>The account details.</returns>
    [HttpGet("{accountNumber}")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAccount(Guid accountNumber)
    {
        var account = await _service.GetAccount(new AccountRequest(accountNumber));
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
    /// <returns>The updated balance.</returns>
    [HttpPost("{accountNumber}/deposits")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> MakeDeposit(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = await _service.MakeDeposit(new TransactionRequest()
        {
            SenderAccountNumber = accountNumber,
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
    /// <returns>The updated balance.</returns>
    [HttpPost("{accountNumber}/withdrawals")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> MakeWithdrawal(Guid accountNumber,[FromBody] TransactionRequest request)
    {
        var account = await _service.MakeWithdraw(new TransactionRequest()
        {
            SenderAccountNumber = accountNumber,
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
    /// <returns>The updated sender balance.</returns>
    [HttpPost("transfers")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> MakeTransfer([FromBody] TransactionRequest request)
    {
        var account = await _service.MakeTransfer(new TransactionRequest()
        {
            SenderAccountNumber = request.SenderAccountNumber,
            ReceiverAccountNumber = request.ReceiverAccountNumber,
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
    /// <param name="currency">The currencies to convert the balance to.</param>
    /// <returns>The list of the converted balance.</returns>
    [HttpGet("{accountNumber}/balances")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status404NotFound)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> CheckConvertedBalance(Guid accountNumber, [FromQuery] string currency)
    {
        var response = await _service.GetConvertedBalanceAsync(
            new AccountRequest(accountNumber),
            new CurrencyRequest { Currency = currency });

        if (!response.IsSuccess)
        {
            return StatusCode(response.HttpStatusCode, response);
        }
        
        return Ok(response.Result);
    }
}