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