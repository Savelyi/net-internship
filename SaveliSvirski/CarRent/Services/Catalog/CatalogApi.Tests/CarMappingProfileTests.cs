using AutoMapper;
using Mapper.MapperProfiles;
using Xunit;

namespace CatalogApi.Tests
{
    public class CarMappingProfileTests
    {
        [Fact]
        public void ValidateMappingConfigurationTest()
        {
            var mapperConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile(new CarMappingProfile());
                });

            IMapper mapper = new AutoMapper.Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}