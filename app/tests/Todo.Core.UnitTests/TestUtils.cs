using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Todo.Core.Data;

namespace Todo.Core.UnitTests;

public static class TestUtils
{
    public static IApplicationDbContext GetInMemoryContext() => GetInMemoryContext([]);

    public static IApplicationDbContext GetInMemoryContext(IInterceptor[] interceptors)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        if (interceptors.Length > 0)
        {
            builder.AddInterceptors(interceptors);
        }

        return new ApplicationDbContext(builder.Options);
    }

    public static IProperty GetProperty<T>(this ApplicationDbContext context, string name)
    {
        var entityType = context.Model.FindEntityType(typeof(T))!;
        return entityType.FindProperty(name)!;
    }

    public static INavigation GetNavigation<T>(this ApplicationDbContext context, string name)
    {
        var entityType = context.Model.FindEntityType(typeof(T))!;
        return entityType.FindNavigation(name)!;
    }
}
