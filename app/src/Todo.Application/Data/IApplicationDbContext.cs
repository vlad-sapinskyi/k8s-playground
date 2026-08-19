using Microsoft.EntityFrameworkCore;
using Todo.Application.Data.Entities;

namespace Todo.Application.Data;

public interface IApplicationDbContext
{
    DbSet<TodoListEntity> TodoLists { get; }

    DbSet<TodoItemEntity> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
