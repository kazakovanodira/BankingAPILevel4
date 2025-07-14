using System.ComponentModel.DataAnnotations;

namespace banking_api_repo.Models.Requests;

public class AccountRequest
{
    public Guid AccountId { get; set; }
}