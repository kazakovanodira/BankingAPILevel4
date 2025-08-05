using BankingAPILevel4.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPILevel4.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();
        
    }
}