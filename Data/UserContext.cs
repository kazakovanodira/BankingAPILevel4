using banking_api_repo.Models;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();
    }
}