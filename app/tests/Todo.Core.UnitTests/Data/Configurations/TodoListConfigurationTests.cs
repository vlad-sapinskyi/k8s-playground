using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Data.Entities;
using Todo.Core.UnitTests.AutoFixture.Attributes;
using Xunit;

namespace Todo.Core.UnitTests.Data.Configurations;

public class TodoListConfigurationTests
{
    [Theory, AutoMoqData]
    public void Title_HasExpectedMaxLength(ApplicationDbContext context)
    {
        var property = context.GetProperty<TodoListEntity>(nameof(TodoListEntity.Title));

        Assert.NotNull(property);
        Assert.Equal(200, property.GetMaxLength());
    }

    [Theory, AutoMoqData]
    public void Title_IsRequired(ApplicationDbContext context)
    {
        var property = context.GetProperty<TodoListEntity>(nameof(TodoListEntity.Title));

        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }

    [Theory, AutoMoqData]
    public void Colour_IsConvertedToInt(ApplicationDbContext context)
    {
        var property = context.GetProperty<TodoListEntity>(nameof(TodoListEntity.Colour));

        Assert.NotNull(property);
        Assert.Equal(typeof(int), property.GetProviderClrType());
    }

    [Theory, AutoMoqData]
    public void Colour_IsRequired(ApplicationDbContext context)
    {
        var property = context.GetProperty<TodoListEntity>(nameof(TodoListEntity.Colour));

        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }

    [Theory, AutoMoqData]
    public void Items_NavigationIsConfigured(ApplicationDbContext context)
    {
        var navigation = context.GetNavigation<TodoListEntity>(nameof(TodoListEntity.Items));

        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
    }

    [Theory, AutoMoqData]
    public void Items_ForeignKey_UsesListId(ApplicationDbContext context)
    {
        var navigation = context.GetNavigation<TodoListEntity>(nameof(TodoListEntity.Items));

        Assert.NotNull(navigation);
        var foreignKey = navigation.ForeignKey;

        var item = Assert.Single(foreignKey.Properties);
        Assert.Equal(nameof(TodoItemEntity.ListId), item.Name);
    }

    [Theory, AutoMoqData]
    public void Items_OnDelete_IsCascade(ApplicationDbContext context)
    {
        var navigation = context.GetNavigation<TodoListEntity>(nameof(TodoListEntity.Items));

        Assert.NotNull(navigation);
        Assert.Equal(DeleteBehavior.Cascade, navigation.ForeignKey.DeleteBehavior);
    }
}
