using System.Text.Json.Serialization;

namespace Forum.BuildingBlocks.Base;

/// <summary>
/// Represents an operation result without data.
/// Fully null-safe.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; }

    internal Result(bool isSuccess, string error)
    {
        if (string.IsNullOrWhiteSpace(error) && !isSuccess)
            throw new InvalidOperationException("Failure result must contain an error message.");

        if (!string.IsNullOrWhiteSpace(error) && isSuccess)
            throw new InvalidOperationException("Success result cannot contain an error message.");

        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static Result Success() => new Result(true, string.Empty);

    public static Result Failure(string error)
        => new Result(false, error ?? "Unknown error.");
}

/// <summary>
/// Represents an operation result (with data).
/// Fully null-safe.
/// </summary>
public sealed class Result<T> : Result where T : notnull
{
    public T Value { get; }

    private Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        if (isSuccess && value == null)
            throw new InvalidOperationException("Success result requires a non-null value.");

        Value = value;
    }

    public static Result<T> Success(T value)
        => new Result<T>(value, true, string.Empty);

    public static new Result<T> Failure(string error)
        => new Result<T>(default!, false, error ?? "Unknown error.");
}

/// <summary>
/// Extension methods to enhance Result functionality.
/// Null-safe.
/// </summary>
public static class ResultExtensions
{
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string error)
        where T : notnull
    {
        if (!result.IsSuccess)
            return result;

        return predicate(result.Value)
            ? result
            : Result<T>.Failure(error);
    }
}