using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todo.Core.Common;
using Todo.Core.Data;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Entities;
using Todo.Core.Data.Enums;

namespace Todo.Core.Services;

public class TodoItemService(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<TodoItemDto> validator) : ITodoItemService
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
            return Result<TodoItemDto>.Failure(validation.Errors.Select(e => e.ErrorMessage));

        var listExists = await context.TodoLists.AnyAsync(x => x.Id == dto.ListId, cancellationToken);
        if (!listExists) 
            return Result<TodoItemDto>.Failure($"List {dto.ListId} does not exist.");

        var entity = new TodoItemEntity
        {
            ListId = dto.ListId,
            Title = dto.Title.Trim(),
            Note = dto.Note,
            Priority = (PriorityLevel)dto.Priority,
            Done = false
        };
        context.TodoItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<TodoItemDto>.Success(mapper.Map<TodoItemDto>(entity));
    }

    public async Task<Result> UpdateAsync(int id, TodoItemDto dto, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid) 
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage));

        var entity = await context.TodoItems.FindAsync([id], cancellationToken);
        if (entity is null) 
            return Result.Failure("Item not found.");

        entity.Title = dto.Title.Trim();
        entity.Note = dto.Note;
        entity.Priority = (PriorityLevel)dto.Priority;
        entity.Done = dto.Done;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([id], cancellationToken);
        if (entity is null) 
            return Result.Failure("Item not found.");

        context.TodoItems.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
