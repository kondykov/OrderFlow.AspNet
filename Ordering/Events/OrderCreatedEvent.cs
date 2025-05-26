using MediatR;

namespace OrderFlow.Ordering.Events;

public class OrderCreatedEvent : INotification
{
    public int OrderId { get; set; }
}