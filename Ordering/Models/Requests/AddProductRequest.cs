namespace OrderFlow.Ordering.Models.Requests;

public class AddProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = false;
}