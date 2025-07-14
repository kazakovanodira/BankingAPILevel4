using banking_api_repo.Models;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Data;

public class AccountsContext : DbContext
{
    public AccountsContext(DbContextOptions<AccountsContext> options) : base(options) { }
    
    public DbSet<Account> Accounts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();
    }
}