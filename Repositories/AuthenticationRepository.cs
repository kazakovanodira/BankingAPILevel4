using BankingAPILevel4.Data;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPILevel4.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly UserContext _context;
    
    public AuthenticationRepository(UserContext context)
    {
        _context = context;
    }
    
    public async Task<UserCredential> Add(UserCredential userCredential)
    {
        _context.UserCredentials.Add(userCredential);
        await _context.SaveChangesAsync();
        return userCredential;
    }

    public async Task<UserCredential?> Get(string username) =>
        await _context.UserCredentials.FirstOrDefaultAsync(u => u.Username == username);
    
}