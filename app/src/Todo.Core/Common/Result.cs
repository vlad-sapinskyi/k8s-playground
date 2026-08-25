namespace Todo.Core.Common;

public record Result(bool Succeeded, IReadOnlyCollection<string> Errors)
{
    public static Result Success() => new(true, []);

    public static Result Failure(params IEnumerable<string> errors) => new(false, [.. errors]);
}

public record Result<T>(bool Succeeded, T? Value, IReadOnlyCollection<string> Errors)
{
    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(params IEnumerable<string> errors) => new(false, default, [.. errors]);
}
