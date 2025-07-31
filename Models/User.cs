using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace banking_api_repo.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public Guid AccountNumber { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "Name is required."), MinLength(2), MaxLength(50)]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Username is required."), MinLength(5), MaxLength(15)]    
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8}$", 
        ErrorMessage = "Password must meet requirements")]
    public string Password { get; set; }

    public decimal Balance { get; set; } = 0;

    [Required]
    [AllowedValues("Admin", "User")]
    public string Role { get; set; } = "User";

}

public enum Role{
    Admin, 
    User
}