using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Shared.Models.Ordering;

public class Order : TimeStamps
{
    [Key] public int Id { get; set; }
    public string UserId { get; set; }
    [JsonIgnore] public User User { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.New;
    public List<OrderItem> OrderItems { get; set; } = [];
}