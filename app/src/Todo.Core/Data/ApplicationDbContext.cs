using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Todo.Core.Data.Entities;
using Todo.Core.Identity;

namespace Todo.Core.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<TodoListEntity> TodoLists => Set<TodoListEntity>();

    public DbSet<TodoItemEntity> TodoItems => Set<TodoItemEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
