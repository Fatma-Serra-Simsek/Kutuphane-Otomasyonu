namespace Library.Business.Results;

public class ServiceResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }

    public static ServiceResult Ok() => new() { Succeeded = true };

    public static ServiceResult Fail(string error) => new()
    {
        Succeeded = false,
        Error = error
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; init; }

    public static ServiceResult<T> Ok(T value) => new()
    {
        Succeeded = true,
        Value = value
    };

    public new static ServiceResult<T> Fail(string error) => new()
    {
        Succeeded = false,
        Error = error
    };
}
