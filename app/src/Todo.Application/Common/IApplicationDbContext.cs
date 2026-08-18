using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;

namespace Todo.Application.Common;

public interface IApplicationDbContext
{
    DbSet<TodoListEntity> TodoLists { get; }
    DbSet<TodoItemEntity> TodoItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
