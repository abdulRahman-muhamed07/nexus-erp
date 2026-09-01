using AutoMapper;
using XeoTechErp.Application.Mapping;

namespace XeoTechErp.Tests;

public sealed class AutoMapperConfigurationTests
{
    [Fact]
    public void Application_mapping_configuration_is_valid()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationMappingProfile>());
        configuration.AssertConfigurationIsValid();
    }
}
