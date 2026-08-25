using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Todo.Core.Data.Entities;
using Todo.Core.Data.Enums;
using Todo.Core.Identity;

namespace Todo.Core.Data;

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context, 
    UserManager<ApplicationUser> userManager, 
    RoleManager<IdentityRole> roleManager)
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
            // Default roles
            var adminRole = new IdentityRole("Admin");
            if (roleManager.Roles.All(r => r.Name != adminRole.Name))
                await roleManager.CreateAsync(adminRole);

            // Default users
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@localhost"
            };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                await userManager.CreateAsync(adminUser, "!Qwerty1");
                if (!string.IsNullOrWhiteSpace(adminRole.Name))
                    await userManager.AddToRolesAsync(adminUser, [adminRole.Name]);
            }

            // Default data
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
