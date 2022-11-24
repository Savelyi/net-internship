using AutoMapper;
using Mapper;
using Xunit;

namespace RentApi.Tests
{
    public class RentMapperProfileTests
    {
        [Fact]
        public void ValidateMappingConfigurationTest()
        {
            var mapperConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile(new RentMapperProfile());
                });

            IMapper mapper = new AutoMapper.Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}