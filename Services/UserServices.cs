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

        await _accountRepository.AddAccount(user);
        await _authenticationRepository.Add(userCredentials);
        
        return new ApiResponse<AccountDto>
        {
            Result = _mapper.Map<AccountDto>(await _accountRepository.AddAccount(user)),
            HttpStatusCode = 201
        };
    }
}