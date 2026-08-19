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

public class TodoListService(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<TodoListDto> validator) : ITodoListService
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
            return Result<TodoListDto>.Failure(validation.Errors.Select(e => e.ErrorMessage));

        var entity = new TodoListEntity
        {
            Title = list.Title.Trim(),
            Colour = Enum.Parse<Colour>(list.Colour)
        };

        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<TodoListDto>.Success(mapper.Map<TodoListDto>(entity));
    }

    public async Task<Result> UpdateAsync(int id, TodoListDto list, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(list, cancellationToken);
        if (!validation.IsValid) 
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage));

        var entity = await context.TodoLists.FindAsync([id], cancellationToken);
        if (entity is null) 
            return Result.Failure("List not found.");

        entity.Title = list.Title.Trim();
        entity.Colour = Enum.Parse<Colour>(list.Colour);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoLists.FindAsync([id], cancellationToken);
        if (entity is null) 
            return Result.Failure("List not found.");

        context.TodoLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
