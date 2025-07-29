using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace banking_api_repo.Models;

public class User : IdentityUser
{
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Balance { get; set; }

}