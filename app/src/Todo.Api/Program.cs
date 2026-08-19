
using Todo.Api;
using Todo.Api.Common;
using Todo.Application;
using Todo.Infrastructure;
using Todo.Infrastructure.Data;
using Todo.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddApiServices();

var app = builder.Build();


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}
else 
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(static builder =>
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.UseFileServer();

app.MapOpenApi();

app.UseExceptionHandler(options => { });

app.MapIdentityApi<ApplicationUser>();

app.MapEndpoints(typeof(Program).Assembly);


app.Run();
