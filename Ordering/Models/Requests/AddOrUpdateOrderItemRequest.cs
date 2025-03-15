namespace OrderFlow.Ordering.Models.Requests;

public class AddOrUpdateOrderItemRequest
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}