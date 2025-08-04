using System.ComponentModel.DataAnnotations;

namespace BankingAPILevel4.Models.Requests;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Name is required."), MinLength(2), MaxLength(50)]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Username is required."), MinLength(5), MaxLength(15)]    
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])$", 
    //    ErrorMessage = "Password must meet requirements")]
    public string Password { get; set; }
    
    [Required]
    [AllowedValues("Admin", "User")]
    public string Role { get; set; } = "User";
}