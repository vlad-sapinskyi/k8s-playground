using Ardalis.GuardClauses;

namespace Todo.Api.Common;

public static class MethodInfoExtensions
{
    private static readonly char[] AnonymousMethodChars = ['<', '>'];

    public static void AnonymousMethod(this IGuardClause guardClause, Delegate input)
    {
        if (input.Method.Name.Any(AnonymousMethodChars.Contains))
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
    }
}
