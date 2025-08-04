using BankingAPILevel4.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAPILevel4.Data;

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