using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
