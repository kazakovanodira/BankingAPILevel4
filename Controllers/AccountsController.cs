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
    /// Retrieves a paginated list of accounts with optional filtering by name and sorting.
    /// </summary>
    /// <param name="name">Optional name to filter accounts.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page (max 10).</param>
    /// <param name="orderBy">The field to sort by (e.g., Name).</param>
    /// <param name="descending">Whether to sort in descending order.</param>
    /// <returns>A paginated and optionally filtered list of accounts.</returns>
    /// <remarks>Supports CSV format via content negotiation by setting Accept header to text/csv.</remarks>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
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
    /// Converts the balance of the specified account into the provided currency or currencies.
    /// </summary>
    /// <param name="accountNumber">The unique account number to retrieve balance for.</param>
    /// <param name="currency">Comma-separated list of target currencies (e.g., USD,EUR).</param>
    /// <returns>The converted balance in the requested currencies.</returns>
    [HttpGet("{accountNumber}/balances")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(ConvertedBalances), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConvertedBalances>), StatusCodes.Status404NotFound)]
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