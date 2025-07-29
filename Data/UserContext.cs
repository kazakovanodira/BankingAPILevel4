using banking_api_repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Data;

public class UserContext : IdentityDbContext<User>
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUser>().Ignore(c => c.Email);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.EmailConfirmed);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.NormalizedEmail);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.PhoneNumber);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.PhoneNumberConfirmed);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.TwoFactorEnabled);
        modelBuilder.Entity<IdentityUser>().Ignore(c => c.NormalizedUserName);
    }
}