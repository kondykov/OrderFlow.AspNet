namespace OrderFlow.Shared.Models.Ordering.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Reason { get; set; }
}