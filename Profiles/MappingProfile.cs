using AutoMapper;
using BankingAPILevel4.Models;
using BankingAPILevel4.Models.Requests;
using BankingAPILevel4.Models.Responses;

namespace BankingAPILevel4.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAccountRequest, User>();
        CreateMap<User, AccountDto>();
        CreateMap<User, LoginResponse>();
    }
}