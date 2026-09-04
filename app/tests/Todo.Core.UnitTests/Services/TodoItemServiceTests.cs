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

public class TodoItemServiceTests
{
    // GetAllAsync

    [Theory, AutoMoqData]
    public async Task GetAllAsync_DatabaseIsEmpty_ReturnsEmptyArray(
        TodoItemService service)
    {
        // Act
        var result = await service.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory, AutoMoqData]
    public async Task GetAllAsync_ReturnsItemsOrderedByTitle(
        ApplicationDbContext context, TodoItemService service, 
        List<TodoItemEntity> entities)
    {
        // Arrange
        context.TodoItems.AddRange(entities);
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
        TodoItemService service, int id)
    {
        // Act
        var result = await service.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Theory, AutoMoqData]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull(
        ApplicationDbContext context, TodoItemService service, 
        List<TodoItemEntity> entities)
    {
        // Arrange
        context.TodoItems.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.GetByIdAsync(int.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Theory, AutoMoqData]
    public async Task GetByIdAsync_ExistingId_ReturnsMatchedList(
        ApplicationDbContext context, TodoItemService service, 
        List<TodoItemEntity> entities)
    {
        // Arrange
        context.TodoItems.AddRange(entities);
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
        [Frozen] Mock<IValidator<TodoItemDto>> validator, ValidationFailure[] validationFailures, 
        ApplicationDbContext context, TodoItemService service, TodoItemDto dto)
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
        Assert.Empty(context.TodoItems);
    }

    [Theory, AutoMoqData]
    public async Task CreateAsync_ListIsNotFound_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoItemDto>> validator, ApplicationDbContext context, 
        TodoItemService service, TodoItemDto dto)
    {
        // Arrange
        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors.First());
        Assert.Empty(context.TodoItems);
    }

    [Theory]
    [InlineAutoMoqData("   test-title   ", "test-title")]
    [InlineAutoMoqData("test title", "test title")]
    public async Task CreateAsync_TrimsTitle_SavesEntityAndReturnsSuccess(
        string inputTitle, string expectedTitle, string note, PriorityLevel priority,
        TodoListEntity listEntity, [Frozen] Mock<IValidator<TodoItemDto>> validator, 
        ApplicationDbContext context, TodoItemService service)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dto = new TodoItemDto
        {
            ListId = listEntity.Id,
            Title = inputTitle,
            Note = note,
            Priority = (int)priority
        };

        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(listEntity.Id, result.Value.ListId);
        Assert.Equal(expectedTitle, result.Value.Title);
        Assert.Equal(note, result.Value.Note);
        Assert.Equal((int)priority, result.Value.Priority);
        Assert.False(result.Value.Done);
        var itemEntity = Assert.Single(context.TodoItems);
        Assert.Equal(listEntity.Id, itemEntity.ListId);
        Assert.Equal(expectedTitle, itemEntity.Title);
        Assert.Equal(note, itemEntity.Note);
        Assert.Equal(priority, itemEntity.Priority);
        Assert.False(itemEntity.Done);
    }

    [Theory]
    [InlineAutoMoqData(0, PriorityLevel.None)]
    [InlineAutoMoqData(1, PriorityLevel.Low)]
    [InlineAutoMoqData(2, PriorityLevel.Medium)]
    [InlineAutoMoqData(3, PriorityLevel.High)]
    [InlineAutoMoqData(int.MaxValue, PriorityLevel.None)]
    public async Task CreateAsync_ParsePriority_SavesEntityAndReturnsSuccess(
        int inputPriority, PriorityLevel expectedPriority, string title, string note,
        TodoListEntity listEntity, [Frozen] Mock<IValidator<TodoItemDto>> validator, 
        ApplicationDbContext context, TodoItemService service)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dto = new TodoItemDto
        {
            ListId = listEntity.Id,
            Title = title,
            Note = note,
            Priority = inputPriority
        };

        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.CreateAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(listEntity.Id, result.Value.ListId);
        Assert.Equal(title, result.Value.Title);
        Assert.Equal(note, result.Value.Note);
        Assert.Equal((int)expectedPriority, result.Value.Priority);
        Assert.False(result.Value.Done);
        var itemEntity = Assert.Single(context.TodoItems);
        Assert.Equal(listEntity.Id, itemEntity.ListId);
        Assert.Equal(title, itemEntity.Title);
        Assert.Equal(note, itemEntity.Note);
        Assert.Equal(expectedPriority, itemEntity.Priority);
        Assert.False(itemEntity.Done);
    }

    // UpdateAsync

    [Theory, AutoMoqData]
    public async Task UpdateAsync_ValidationFails_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoItemDto>> validator, ValidationFailure[] validationFailures,
        ApplicationDbContext context, TodoItemService service, TodoItemDto dto)
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
        Assert.Empty(context.TodoItems);
    }

    [Theory, AutoMoqData]
    public async Task UpdateAsync_EntityIsNotFound_ReturnsFailureWithoutSaving(
        [Frozen] Mock<IValidator<TodoItemDto>> validator, ApplicationDbContext context, 
        TodoItemService service, TodoItemDto dto)
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
        Assert.Empty(context.TodoItems);
    }

    [Theory]
    [InlineAutoMoqData("   test-title   ", "test-title")]
    [InlineAutoMoqData("test title", "test title")]
    public async Task UpdateAsync_TrimsTitle_SavesEntityAndReturnsSuccess(
        string inputTitle, string expectedTitle, string note, bool done, PriorityLevel priority,
        TodoListEntity listEntity, TodoItemEntity itemEntity, [Frozen] Mock<IValidator<TodoItemDto>> validator,
        ApplicationDbContext context, TodoItemService service)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        itemEntity.ListId = listEntity.Id;
        itemEntity.List = listEntity;
        context.TodoItems.Add(itemEntity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dto = new TodoItemDto
        {
            Title = inputTitle,
            Note = note,
            Priority = (int)priority,
            Done = done
        };

        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.UpdateAsync(itemEntity.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        var updatedItemEntity = context.TodoItems.Single(i => i.Id == itemEntity.Id);
        Assert.Equal(listEntity.Id, updatedItemEntity.ListId);
        Assert.Equal(expectedTitle, updatedItemEntity.Title);
        Assert.Equal(note, updatedItemEntity.Note);
        Assert.Equal(priority, updatedItemEntity.Priority);
        Assert.Equal(done, updatedItemEntity.Done);
    }

    [Theory]
    [InlineAutoMoqData(0, PriorityLevel.None)]
    [InlineAutoMoqData(1, PriorityLevel.Low)]
    [InlineAutoMoqData(2, PriorityLevel.Medium)]
    [InlineAutoMoqData(3, PriorityLevel.High)]
    [InlineAutoMoqData(int.MaxValue, PriorityLevel.None)]
    public async Task UpdateAsync_ParsePriority_SavesEntityAndReturnsSuccess(
        int inputPriority, PriorityLevel expectedPriority, string title, string note, bool done,
        TodoListEntity listEntity, TodoItemEntity itemEntity, [Frozen] Mock<IValidator<TodoItemDto>> validator,
        ApplicationDbContext context, TodoItemService service)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        itemEntity.ListId = listEntity.Id;
        itemEntity.List = listEntity;
        context.TodoItems.Add(itemEntity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dto = new TodoItemDto
        {
            Title = title,
            Note = note,
            Priority = inputPriority,
            Done = done
        };

        validator
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await service.UpdateAsync(itemEntity.Id, dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        var updatedItemEntity = context.TodoItems.Single(i => i.Id == itemEntity.Id);
        Assert.Equal(listEntity.Id, updatedItemEntity.ListId);
        Assert.Equal(title, updatedItemEntity.Title);
        Assert.Equal(note, updatedItemEntity.Note);
        Assert.Equal(expectedPriority, updatedItemEntity.Priority);
        Assert.Equal(done, updatedItemEntity.Done);
    }

    // DeleteAsync

    [Theory, AutoMoqData]
    public async Task DeleteAsync_EntityNotFound_ReturnsFailure(TodoItemService service)
    {
        // Act
        var result = await service.DeleteAsync(int.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors.First());
    }

    [Theory, AutoMoqData]
    public async Task DeleteAsync_ExistingEntity_RemovesEntityAndReturnsSuccess(
        ApplicationDbContext context, TodoItemService service, 
        TodoListEntity listEntity, TodoItemEntity itemEntity)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        itemEntity.ListId = listEntity.Id;
        itemEntity.List = listEntity;
        context.TodoItems.Add(itemEntity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.DeleteAsync(itemEntity.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(context.TodoItems);
    }

    [Theory, AutoMoqData]
    public async Task DeleteAsync_ExistingEntity_DoesNotAffectOtherEntities(
        ApplicationDbContext context, TodoItemService service,
        TodoListEntity listEntity, List<TodoItemEntity> itemEntities)
    {
        // Arrange
        context.TodoLists.Add(listEntity);
        foreach (var itemEntity in itemEntities)
        {
            itemEntity.ListId = listEntity.Id;
            itemEntity.List = listEntity;
        }
        context.TodoItems.AddRange(itemEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await service.DeleteAsync(itemEntities[0].Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equivalent(itemEntities.Skip(1).ToArray(), context.TodoItems.ToArray());
    }
}
