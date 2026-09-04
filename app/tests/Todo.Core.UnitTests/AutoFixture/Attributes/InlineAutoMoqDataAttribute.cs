using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit3;
using Todo.Core.UnitTests.AutoFixture.Customizations;

namespace Todo.Core.UnitTests.AutoFixture.Attributes;

public class InlineAutoMoqDataAttribute(params object[] values) 
    : InlineAutoDataAttribute(() => new Fixture()
        .Customize(new AutoMoqCustomization())
        .Customize(new InMemoryContextCustomization())
        .Customize(new MapperCustomization()), values)
{
}
