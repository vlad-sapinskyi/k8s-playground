using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Todo.Application.Data;
using Todo.Application.Identity;
using Todo.Application.Services;

namespace Todo.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDatabaseServices();
        builder.AddIdentityServices();
        builder.AddMappingServices();
        builder.AddValidationServices();

        builder.Services.AddScoped<ITodoListService, TodoListService>();
        builder.Services.AddScoped<ITodoItemService, TodoItemService>();
    }

    private static void AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("TodoDb");

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);
        });

        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
    }

    private static void AddIdentityServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
    }

    private static void AddMappingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg =>
            cfg.AddMaps(Assembly.GetExecutingAssembly()));
    }

    private static void AddValidationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
