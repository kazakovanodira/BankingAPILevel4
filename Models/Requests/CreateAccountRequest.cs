using System.ComponentModel.DataAnnotations;

namespace banking_api_repo.Models.Requests;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Name cannot be empty.")]
    public string Name { get; set; }
}   