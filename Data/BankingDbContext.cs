using BankingAPILevel4.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPILevel4.Data;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.AccountNumber)
            .IsUnique();

        modelBuilder.Entity<UserCredential>()
            .HasIndex(uc => uc.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserCredential)
            .WithOne(uc => uc.User)
            .HasForeignKey<UserCredential>(uc => uc.UserId);

        base.OnModelCreating(modelBuilder);
    }
}