using Todo.Core;
using Todo.Core.Data;
using Todo.Core.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddOpenApi();


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
app.UseExceptionHandler("/error");
app.MapIdentityApi<ApplicationUser>();
app.MapControllers();


app.Run();
