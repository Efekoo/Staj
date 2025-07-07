using AutoMapper;
using Backend.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Backend.Core.DTOs;

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