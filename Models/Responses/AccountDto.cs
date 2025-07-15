namespace banking_api_repo.Models.Responses;

public record AccountDto(Guid AccountId, string Name, decimal Balance);