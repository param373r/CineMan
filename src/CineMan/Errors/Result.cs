namespace CineMan.Errors;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null || !isSuccess && error == null)
        {
            throw new ArgumentException("Result cannot be successful and contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result
{
    public Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        Value = value;
    }

    private readonly T? Value;

    public T GetValue()
    {
        if (IsFailure)
        {
            throw new InvalidOperationException("Cannot get value from failed result.");
        }

        return Value!;
    }
}
