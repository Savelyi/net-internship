using AutoMapper;
using Mapper;
using Xunit;

namespace IdentityApi.Tests
{
    public class UserMappingProfileTests
    {
        [Fact]
        public void ValidateMappingConfigurationTest()
        {
            var mapperConfig = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile(new UserMappingProfile());
                });

            IMapper mapper = new AutoMapper.Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}