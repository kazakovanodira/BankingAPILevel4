using BankingAPILevel4.Models.Entities;

namespace BankingAPILevel4.Interfaces;

public interface IAuthenticationRepository
{
    Task<UserCredential> Add(UserCredential userCredential);
    Task<UserCredential?> Get(string username);
}