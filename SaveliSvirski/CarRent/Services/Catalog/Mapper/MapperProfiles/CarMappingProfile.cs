using System.Linq.Expressions;
using AutoMapper;
using BusinessLogic.Dto;
using CQRS.Commands;
using Data.Models;

namespace Mapper.MapperProfiles
{
    public class CarMappingProfile : Profile
    {
        public CarMappingProfile()
        {
            CreateMap<CarModel, CarToShowDto>()
                .ForMember(c => c.Make,
                    opt => opt.MapFrom(x => x.CarMakeInfo.Make))
                .ForMember(c => c.MakeId,
                    opt => opt.MapFrom(x => x.CarMakeId));
            CreateMap<CarToAddCommand, CarModel>(MemberList.Source)
                .ForSourceMember(e => e.Make, opt => opt.DoNotValidate());
            CreateMap<CarToUpdateCommand, CarModel>(MemberList.Source)
                .ForSourceMember(e => e.Make, opt => opt.DoNotValidate());
        }
    }
}