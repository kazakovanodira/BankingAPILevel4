using System.ComponentModel.DataAnnotations;

namespace banking_api_repo.Models.Requests;

public class TransactionRequest
{
    [Required]
    public string SenderAccId { get; set; }
    public string? ReceiverAccId { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}