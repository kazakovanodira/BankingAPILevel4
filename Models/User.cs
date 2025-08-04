using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingAPILevel4.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid AccountNumber { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }

    public decimal Balance { get; set; } = 0;

    public string Role { get; set; } = "User";
}