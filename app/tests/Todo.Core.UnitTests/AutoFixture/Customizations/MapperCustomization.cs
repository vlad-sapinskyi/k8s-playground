using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Todo.Core.UnitTests.AutoFixture.Customizations;

public class MapperCustomization : ICustomization
{
    public void Customize(IFixture fixture) => fixture.Register(() =>
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(AssemblyMarker).Assembly), 
            NullLoggerFactory.Instance).CreateMapper());
}
