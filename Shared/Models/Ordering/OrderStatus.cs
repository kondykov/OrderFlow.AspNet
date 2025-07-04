using System.ComponentModel;

namespace OrderFlow.Shared.Models.Ordering;

public enum OrderStatus
{
    [Description("Новый")] New,
    [Description("В обработке")] Processing,
    [Description("Ожидает оплаты")] AwaitingPayment,
    [Description("Оплачен")] Paid,
    [Description("Завершён")] Completed,
    [Description("Отменён")] Cancelled,
    [Description("Отклонён")] Rejected
}

public static class OrderStatusExtensions
{
    private static readonly List<OrderStatus> StatusQueue =
    [
        OrderStatus.New,
        OrderStatus.Processing,
        OrderStatus.AwaitingPayment,
        OrderStatus.Paid,
        OrderStatus.Completed
    ];

    public static Order Next(this Order order)
    {
        var currentStatus = order.Status;
        var currentIndex = StatusQueue.IndexOf(currentStatus);

        if (currentIndex == -1) throw new ArgumentException("Смена статуса по очереди для данного заказа невозможна.");

        if (currentIndex == StatusQueue.Count - 1) return order;
        var nextStatus = StatusQueue[currentIndex + 1];
        order.Status = nextStatus;
        return order; 
    }
}