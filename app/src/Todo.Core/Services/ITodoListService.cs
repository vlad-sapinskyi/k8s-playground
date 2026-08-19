using Todo.Core.Common;
using Todo.Core.Data.Dtos;

namespace Todo.Core.Services;

public interface ITodoListService
{
    Task<TodoListDto[]> GetAllAsync(CancellationToken cancellationToken);

    Task<TodoListDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Result<TodoListDto>> CreateAsync(TodoListDto list, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(int id, TodoListDto list, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
