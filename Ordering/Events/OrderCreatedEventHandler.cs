using MediatR;

namespace OrderFlow.Ordering.Events;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("O R D E R I N G    T E S T    E V E N T S");
    }
}