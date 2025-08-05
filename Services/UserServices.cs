using AutoMapper;
using BankingAPILevel4.Hasher;
using BankingAPILevel4.Interfaces;
using BankingAPILevel4.Models.Entities;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;

namespace BankingAPILevel4.Services;

public class UserServices : IUserServices
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly IMapper _mapper;

    public UserServices(IAccountRepository accountRepository,
        IAuthenticationRepository authenticationRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _authenticationRepository = authenticationRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Creates a new user account and stores the user credentials.
    /// </summary>
    /// <param name="createAccountRequest">The registration data including name, username, password, and role.</param>
    /// <returns>Returns the created account details wrapped in an API response.</returns>
    public async Task<ApiResponse<AccountDto>> CreateAccount(CreateAccountRequest createAccountRequest)
    {
        createAccountRequest.Password = Md5Hasher.ComputeHash(createAccountRequest.Password);
        
        var user = _mapper.Map<User>(createAccountRequest);

        var userCredentials = _mapper.Map<UserCredential>(createAccountRequest);

        if (await _authenticationRepository.Get(userCredentials.Username) != null)
        {
            return new ApiResponse<AccountDto>
            {
                ErrorMessage = "Account with this username already exists.",
                HttpStatusCode = 409
            };
        }

        var createdUser = await _accountRepository.AddAccount(user);
        
        userCredentials.UserId = createdUser.Id;
        
        await _authenticationRepository.Add(userCredentials);
        
        return new ApiResponse<AccountDto>
        {
            Result = _mapper.Map<AccountDto>(createdUser),
            HttpStatusCode = 201
        };
    }
}