namespace OrderFlow.Ordering.Models;

public class PaginationResponse<T>
{
    public int Pages { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public T Data { get; set; }
}