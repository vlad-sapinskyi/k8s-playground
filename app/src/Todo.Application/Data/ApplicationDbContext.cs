using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Todo.Application.Data.Entities;
using Todo.Application.Identity;

namespace Todo.Application.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<TodoListEntity> TodoLists => Set<TodoListEntity>();

    public DbSet<TodoItemEntity> TodoItems => Set<TodoItemEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
