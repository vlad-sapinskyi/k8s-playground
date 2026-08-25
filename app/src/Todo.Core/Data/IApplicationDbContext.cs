using Microsoft.EntityFrameworkCore;
using Todo.Core.Data.Entities;

namespace Todo.Core.Data;

public interface IApplicationDbContext
{
    DbSet<TodoListEntity> TodoLists { get; }

    DbSet<TodoItemEntity> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
