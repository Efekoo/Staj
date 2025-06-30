using AutoMapper;
using Backend.API.Dtos;
using Backend.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserRegisterDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
        }
    }
}