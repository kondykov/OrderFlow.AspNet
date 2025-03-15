using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OrderFlow.Shared.Models.Ordering;

public class OrderItem : TimeStamps
{
    [Key] public int Id { get; set; }
    public int OrderId { get; set; }
    [JsonIgnore] public Order Order { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore] public Product Product { get; set; }
    public int Quantity { get; set; }
}