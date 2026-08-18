using Microsoft.Extensions.Logging;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Data;

public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(new TodoListEntity
                {
                    Title = "Tasks",
                    Colour = Colour.Green,
                    Items =
                    {
                        new TodoItemEntity { Title = "Make a todo list 📃" },
                        new TodoItemEntity { Title = "Check off the first item ✅" },
                        new TodoItemEntity { Title = "Realise you've already done two things on the list! 🤯"},
                        new TodoItemEntity { Title = "Reward yourself with a nice, long nap 🏆" },
                    }
                });

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
