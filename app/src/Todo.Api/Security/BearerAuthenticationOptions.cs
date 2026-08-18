using Microsoft.AspNetCore.Authentication;

namespace Todo.Api.Security;

public class BearerAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Token { get; set; } = string.Empty;
}
