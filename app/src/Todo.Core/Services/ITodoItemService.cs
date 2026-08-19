using Todo.Core.Common;
using Todo.Core.Data.Dtos;

namespace Todo.Core.Services;

public interface ITodoItemService
{
    Task<TodoItemDto[]> GetAllAsync(CancellationToken cancellationToken);

    Task<TodoItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Result<TodoItemDto>> CreateAsync(TodoItemDto item, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(int id, TodoItemDto item, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
