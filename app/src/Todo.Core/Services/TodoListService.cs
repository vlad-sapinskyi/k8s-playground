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

public class TodoListService(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<TodoListDto> validator,
    ILogger<TodoListService> logger) : ITodoListService
{
    public async Task<TodoListDto[]> GetAllAsync(CancellationToken cancellationToken) =>
        await context.TodoLists
            .AsNoTracking()
            .ProjectTo<TodoListDto>(mapper.ConfigurationProvider)
            .OrderBy(t => t.Title)
            .ToArrayAsync(cancellationToken);

    public async Task<TodoListDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await context.TodoLists
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<TodoListDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<TodoListDto>> CreateAsync(TodoListDto list, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(list, cancellationToken);
        if (!validation.IsValid)
        {
            logger.LogWarning("Validation failed creating TodoList: {Errors}",
                string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));
            return Result<TodoListDto>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var entity = new TodoListEntity
        {
            Title = list.Title.Trim(),
            Colour = Enum.Parse<Colour>(list.Colour)
        };

        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created TodoList {ListId} with title {Title}", entity.Id, entity.Title);
        return Result<TodoListDto>.Success(mapper.Map<TodoListDto>(entity));
    }

    public async Task<Result> UpdateAsync(int id, TodoListDto list, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(list, cancellationToken);
        if (!validation.IsValid)
        {
            logger.LogWarning("Validation failed updating TodoList {ListId}: {Errors}",
                id, string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var entity = await context.TodoLists.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Attempted to update non-existent TodoList {ListId}", id);
            return Result.Failure("List not found.");
        }

        entity.Title = list.Title.Trim();
        entity.Colour = Enum.Parse<Colour>(list.Colour);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated TodoList {ListId}", id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoLists.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Attempted to delete non-existent TodoList {ListId}", id);
            return Result.Failure("List not found.");
        }

        context.TodoLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted TodoList {ListId}", id);
        return Result.Success();
    }
}
