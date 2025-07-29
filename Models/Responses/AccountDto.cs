namespace banking_api_repo.Models.Responses;

public record AccountDto
{
    public string Name { get; set; }
    public string Id { get; set; }
    public decimal Balance { get; set; }
}