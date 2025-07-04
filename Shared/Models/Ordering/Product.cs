using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OrderFlow.Shared.Models.Ordering;

public class Product
{
    [Key] public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    [JsonIgnore] public List<OrderItem> OrderItems { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public bool IsSellable { get; set; } = false;
    [Column(TypeName = "jsonb")] public List<ProductComponent> Components { get; set; } = [];
    public double Weight { get; set; } = 0;
}