namespace OrderFlow.Shared.Models;

public class ApiErrorResponse
{
    public required int Code { get; set; }
    public required string Message { get; set; }
}