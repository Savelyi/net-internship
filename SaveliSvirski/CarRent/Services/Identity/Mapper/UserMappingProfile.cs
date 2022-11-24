using AutoMapper;
using BusinessLogic.Dto;
using Microsoft.AspNetCore.Identity;

namespace Mapper
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserToSignUpDto, IdentityUser>()
                .ForMember(e => e.Email, u => u.MapFrom(x => x.Email))
                .ForAllOtherMembers(e=>e.Ignore());
        }
    }
}