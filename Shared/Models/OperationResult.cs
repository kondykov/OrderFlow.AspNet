namespace OrderFlow.Shared.Models;

public class OperationResult : OperationResult<string>
{
}

public class OperationResult<T>
{
    public virtual T? Data { get; init; }
    public virtual string? Error { get; init; }
    public virtual bool IsSuccessful => Error == null;
}