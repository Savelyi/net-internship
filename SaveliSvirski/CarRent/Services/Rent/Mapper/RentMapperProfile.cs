using AutoMapper;
using BusinessLogic.Dto;
using Data.Models;

namespace Mapper
{
    public class RentMapperProfile : Profile
    {
        public RentMapperProfile()
        {
            CreateMap<Rent, RentToShowDto>();
        }
    }
}