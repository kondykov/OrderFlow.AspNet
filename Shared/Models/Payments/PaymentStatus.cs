using System.ComponentModel;

namespace OrderFlow.Shared.Models.Payments;

public enum PaymentStatus
{
    [Description("Отменён")] Cancelled,
    [Description("Ожидает")] Awaiting,
    [Description("Завершён")] Done
}