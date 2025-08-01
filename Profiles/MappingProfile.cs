using AutoMapper;
using banking_api_repo.Models;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAccountRequest, User>();
        CreateMap<User, AccountDto>();
        CreateMap<User, LoginResponse>();
    }
}