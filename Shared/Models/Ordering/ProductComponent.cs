using System.Text.Json.Serialization;

namespace OrderFlow.Shared.Models.Ordering;

public class ProductComponent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}