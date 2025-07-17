using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banking_api_repo.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid AccountNumber { get; set; } = Guid.Empty;
    [Required]
    public decimal Balance { get; set; } = 0;
}