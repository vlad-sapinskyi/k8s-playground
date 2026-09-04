using Todo.Core.Data;
using Todo.Core.Data.Entities;
using Todo.Core.UnitTests.AutoFixture.Attributes;
using Xunit;

namespace Todo.Core.UnitTests.Data.Configurations;

public class TodoItemConfigurationTests
{
    [Theory, AutoMoqData]
    public void Title_HasExpectedMaxLength(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Title));

        // Assert
        Assert.NotNull(property);
        Assert.Equal(200, property.GetMaxLength());
    }

    [Theory, AutoMoqData]
    public void Title_IsRequired(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Title));

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }

    [Theory, AutoMoqData]
    public void Note_HasExpectedMaxLength(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Note));

        // Assert
        Assert.NotNull(property);
        Assert.Equal(1000, property.GetMaxLength());
    }

    [Theory, AutoMoqData]
    public void Note_IsOptional(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Note));

        // Assert
        Assert.NotNull(property);
        Assert.True(property.IsNullable);
    }

    [Theory, AutoMoqData]
    public void Priority_IsConvertedToInt(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Priority));

        // Assert
        Assert.NotNull(property);
        Assert.Equal(typeof(int), property.GetProviderClrType());
    }

    [Theory, AutoMoqData]
    public void Priority_IsRequired(ApplicationDbContext context)
    {
        // Arrange
        var property = context.GetProperty<TodoItemEntity>(nameof(TodoItemEntity.Priority));

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }
}
