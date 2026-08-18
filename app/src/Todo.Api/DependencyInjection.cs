using Microsoft.AspNetCore.Mvc;
using Todo.Api.Common;
using Todo.Api.Security;

namespace Todo.Api;

public static class DependencyInjection
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication(BearerAuthenticationHandler.SchemeName)
            .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(
                BearerAuthenticationHandler.SchemeName, options =>
                    builder.Configuration.GetSection("BearerAuth").Bind(options));

        builder.Services.AddAuthorization();

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<ApiExceptionOperationTransformer>();
        });

        builder.Services.AddCors();
    }
}
