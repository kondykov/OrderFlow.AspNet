using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OrderFlow.Shared.Models.Ordering;

public class Product
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    [JsonIgnore] public List<OrderItem> OrderItems { get; set; } = [];
    public bool IsActive { get; set; } = true;
}