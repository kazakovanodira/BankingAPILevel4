using banking_api_repo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace banking_api_repo.Data;

public class UserContext : IdentityDbContext<User>
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }
}