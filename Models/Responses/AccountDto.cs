namespace BankingAPILevel4.Models.Responses;

public record AccountDto
{
    public string Name { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
}