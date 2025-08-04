using System.ComponentModel.DataAnnotations;

namespace BankingAPILevel4.Models.Requests;

public class TransactionRequest
{
    [Required]
    public Guid SenderAccountNumber { get; set; }
    
    public Guid? ReceiverAccountNumber { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}