using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Core.Common;
using Todo.Core.Data;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Entities;
using Todo.Core.Data.Enums;

namespace Todo.Core.Services;

public class TodoItemService(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<TodoItemDto> validator,
    ILogger<TodoItemService> logger) : ITodoItemService
{
    public async Task<TodoItemDto[]> GetAllAsync(CancellationToken cancellationToken) =>
        await context.TodoItems
            .AsNoTracking()
            .ProjectTo<TodoItemDto>(mapper.ConfigurationProvider)
            .OrderBy(t => t.Title)
            .ToArrayAsync(cancellationToken);

    public async Task<TodoItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await context.TodoItems
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<TodoItemDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<TodoItemDto>> CreateAsync(TodoItemDto dto, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            var errorMessages = validation.Errors.Select(e => e.ErrorMessage);
            logger.LogWarning("Validation failed creating TodoItem: {@Errors}", [.. errorMessages]);
            return Result<TodoItemDto>.Failure(errorMessages);
        }

        var listExists = await context.TodoLists.AnyAsync(x => x.Id == dto.ListId, cancellationToken);
        if (!listExists)
        {
            logger.LogWarning("Attempted to create TodoItem for non-existent List {ListId}", dto.ListId);
            return Result<TodoItemDto>.Failure($"List {dto.ListId} does not exist.");
        }

        var entity = new TodoItemEntity
        {
            ListId = dto.ListId,
            Title = dto.Title.Trim(),
            Note = dto.Note,
            Priority = Enum.IsDefined(typeof(PriorityLevel), dto.Priority)
                ? (PriorityLevel)dto.Priority
                : PriorityLevel.None,
            Done = false
        };
        context.TodoItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created TodoItem {ItemId} in List {ListId}", entity.Id, entity.ListId);
        return Result<TodoItemDto>.Success(mapper.Map<TodoItemDto>(entity));
    }

    public async Task<Result> UpdateAsync(int id, TodoItemDto dto, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            var errorMessages = validation.Errors.Select(e => e.ErrorMessage);
            logger.LogWarning("Validation failed updating TodoItem {ItemId}: {@Errors}",
               id, errorMessages.ToArray());
            return Result.Failure(errorMessages);
        }

        var entity = await context.TodoItems.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Attempted to update non-existent TodoItem {ItemId}", id);
            return Result.Failure("Item not found.");
        }

        entity.Title = dto.Title.Trim();
        entity.Note = dto.Note;
        entity.Priority = Enum.IsDefined(typeof(PriorityLevel), dto.Priority)
                ? (PriorityLevel)dto.Priority
                : PriorityLevel.None;
        entity.Done = dto.Done;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated TodoItem {ItemId}", id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Attempted to delete non-existent TodoItem {ItemId}", id);
            return Result.Failure("Item not found.");
        }

        context.TodoItems.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted TodoItem {ItemId}", id);
        return Result.Success();
    }
}
