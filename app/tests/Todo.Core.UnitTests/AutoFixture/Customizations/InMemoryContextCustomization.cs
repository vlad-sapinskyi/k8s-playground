using AutoFixture;
using Todo.Core.Data;

namespace Todo.Core.UnitTests.AutoFixture.Customizations;

public class InMemoryContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var context = (ApplicationDbContext)TestUtils.GetInMemoryContext();
        fixture.Inject(context);
        fixture.Inject<IApplicationDbContext>(context);
    }
}
