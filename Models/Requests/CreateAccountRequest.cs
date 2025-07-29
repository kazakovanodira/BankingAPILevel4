using System.ComponentModel.DataAnnotations;

namespace banking_api_repo.Models.Requests;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Name cannot be empty.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    public string AccountNumber;
    public decimal Balance = 0;
}   