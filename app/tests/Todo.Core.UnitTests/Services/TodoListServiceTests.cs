using AutoFixture.Xunit3;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Todo.Core.Data;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Entities;
using Todo.Core.Data.Enums;
using Todo.Core.Services;
using Todo.Core.UnitTests.AutoFixture.Attributes;
using Xunit;

namespace Todo.Core.UnitTests.Services;

public class TodoListServiceTests
{
    // GetAllAsync

    [Theory, AutoMoqData]
    public async Task GetAllAsync_DatabaseIsEmpty_ReturnsEmptyArray(
        TodoListService service)
    {
        // Act
        var result = await service.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory, AutoMoqData]
    public async Task GetAllAsync_ReturnsListsOrderedByTitle(
        ApplicationDbContext context, TodoListService service, 
        List<TodoListEntity> entities)
    {
        // Arrange
        context.TodoLists.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var expected = entities.Select(t => t.Title).OrderBy(t => t).ToArray();

        // Act
        var result = await service.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected, result.Select(r => r.Title).ToArray());
    }

    // GetByIdAsync

    [Theory, AutoMoqData]
    public async Task GetByIdAsync_DatabaseIsEmpty_ReturnsNull(
        TodoListService service, int id)
    {
        // Act
        var result = await service.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Theory, AutoMoqData]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull(
        ApplicationDbContext context, TodoListService service, 
        List<TodoListEntity> entities)
    {
        // Arrange
        context.TodoLists.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.GetByIdAsync(int.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Theory, AutoMoqData]
    public async Task GetByIdAsync_ExistingId_ReturnsMatchedList(
        ApplicationDbContext context, TodoListService service, 
        List<TodoListEntity> entities)
    {
        // Arrange
        context.TodoLists.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.GetByIdAsync(entities[0].Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entities[0].Id, result.Id);
        Assert.Equal(entities[0].Title, result.Title);
    }

    // CreateAsync

    [Theory, AutoMoqData]
    public async Task CreateAsync_ValidationFails_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoListDto>> validator, ValidationFailure[] validationFailures, 
        ApplicationDbContext context, TodoListService service, TodoListDto dto)
    {
        // Arrange
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(validationFailures.Select(e => e.ErrorMessage).ToArray(), result.Errors.ToArray());
        Assert.Empty(context.TodoLists);
    }

    [Theory]
    [InlineAutoMoqData("   test-title   ", "test-title")]
    [InlineAutoMoqData("test title", "test title")]
    public async Task CreateAsync_TrimsTitle_SavesEntityAndReturnsSuccess(
        string inputTitle, string expectedTitle, Colour colour,
        [Frozen] Mock<IValidator<TodoListDto>> validator, 
        ApplicationDbContext context, TodoListService service)
    {
        // Arrange
        var dto = new TodoListDto
        {
            Title = inputTitle,
            Colour = colour.ToString(),
        };
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedTitle, result.Value.Title);
        var entity = Assert.Single(context.TodoLists);
        Assert.Equal(expectedTitle, entity.Title);
    }

    [Theory]
    [InlineAutoMoqData("Red", Colour.Red)]
    [InlineAutoMoqData("red", Colour.Grey)]
    [InlineAutoMoqData("qwerty", Colour.Grey)]
    public async Task CreateAsync_ParseColour_SavesEntityAndReturnsSuccess(
        string inputColour, Colour expectedColour, string title, 
        [Frozen] Mock<IValidator<TodoListDto>> validator, ApplicationDbContext context, 
        TodoListService service)
    {
        // Arrange
        var dto = new TodoListDto 
        {
            Title = title,
            Colour = inputColour
        };
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedColour.ToString(), result.Value.Colour);
        var entity = Assert.Single(context.TodoLists);
        Assert.Equal(expectedColour, entity.Colour);
    }

    // UpdateAsync

    [Theory, AutoMoqData]
    public async Task UpdateAsync_ValidationFails_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoListDto>> validator, ValidationFailure[] validationFailures,
        ApplicationDbContext context, TodoListService service, TodoListDto dto)
    {
        // Arrange
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await service.UpdateAsync(dto.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(validationFailures.Select(e => e.ErrorMessage).ToArray(), result.Errors.ToArray());
        Assert.Empty(context.TodoLists);
    }

    [Theory, AutoMoqData]
    public async Task UpdateAsync_EntityIsNotFound_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoListDto>> validator, ApplicationDbContext context, 
        TodoListService service, TodoListDto dto)
    {
        // Arrange
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.UpdateAsync(dto.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors.First());
        Assert.Empty(context.TodoLists);
    }

    [Theory]
    [InlineAutoMoqData("   test-title   ", "test-title")]
    [InlineAutoMoqData("test title", "test title")]
    public async Task UpdateAsync_TrimsTitle_SavesEntityAndReturnsSuccess(
        string inputTitle, string expectedTitle, Colour colour,
        [Frozen] Mock<IValidator<TodoListDto>> validator, ApplicationDbContext context, 
        TodoListService service, TodoListEntity entity)
    {
        // Arrange
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var dto = new TodoListDto
        {
            Title = inputTitle,
            Colour = colour.ToString(),
        };
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.UpdateAsync(entity.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        var updatedEntity = Assert.Single(context.TodoLists);
        Assert.Equal(expectedTitle, entity.Title);
    }

    [Theory]
    [InlineAutoMoqData("Red", Colour.Red)]
    [InlineAutoMoqData("red", Colour.Grey)]
    [InlineAutoMoqData("qwerty", Colour.Grey)]
    public async Task UpdateAsync_ParseColour_SavesEntityAndReturnsSuccess(
        string inputColour, Colour expectedColour, string title,
        [Frozen] Mock<IValidator<TodoListDto>> validator, ApplicationDbContext context,
        TodoListService service, TodoListEntity entity)
    {
        // Arrange
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dto = new TodoListDto
        {
            Title = title,
            Colour = inputColour
        };
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.UpdateAsync(entity.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        var updatedEntity = Assert.Single(context.TodoLists);
        Assert.Equal(expectedColour, entity.Colour);
    }

    // DeleteAsync

    [Theory, AutoMoqData]
    public async Task DeleteAsync_EntityNotFound_ReturnsFailure(TodoListService service)
    {
        // Act
        var result = await service.DeleteAsync(int.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors.First());
    }

    [Theory, AutoMoqData]
    public async Task DeleteAsync_ExistingEntity_RemovesEntityAndReturnsSuccess(
        ApplicationDbContext context, TodoListService service, TodoListEntity entity)
    {
        // Arrange
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.DeleteAsync(entity.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(context.TodoLists);
    }
    [Theory, AutoMoqData]
    public async Task DeleteAsync_ExistingEntity_DoesNotAffectOtherEntities(
        ApplicationDbContext context, TodoListService service, List<TodoListEntity> entities)
    {
        // Arrange
        context.TodoLists.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.DeleteAsync(entities[0].Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equivalent(entities.Skip(1).ToArray(), context.TodoLists.ToArray());
    }
}
