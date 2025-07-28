using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace banking_api_repo.Models;

public class User : IdentityUser
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid AccountNumber { get; set; } = Guid.Empty;
    [Required]
    public decimal Balance { get; set; } = 0;

}