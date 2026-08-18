using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Todo.Api.Security;

public class BearerAuthenticationHandler(IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<BearerAuthenticationOptions>(options, logger, encoder)
{
    public const string SchemeName = "Bearer";
    public const string ClaimName = "User";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        var header = authorizationHeader.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        var token = header["Bearer ".Length..].Trim();
        if (token != Options.Token)
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, ClaimName) };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
