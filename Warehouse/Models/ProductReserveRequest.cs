namespace OrderFlow.Stock.Models;

public class ProductReserveRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}