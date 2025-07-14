namespace banking_api_repo.Models.Responses;

public record AccountResponse(Guid AccountId, string Name, decimal Balance);