namespace OrderFlow.Ordering.Models.Requests;

public class UpdateOrderRequest
{
    public int Id { get; set; }
    public string OrderStatus { get; set; }
    public string? Reason { get; set; } = null;
}