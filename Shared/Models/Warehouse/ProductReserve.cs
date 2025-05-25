using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Shared.Models.Warehouse;

public class ProductReserve : TimeStamps
{
    [Key] public int Id { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore] public Product? Product { get; set; }
    public double Quantity { get; set; }
}