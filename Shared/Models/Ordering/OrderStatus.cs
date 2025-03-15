namespace OrderFlow.Shared.Models.Ordering;

public enum OrderStatus
{
    New,
    Processing,
    AwaitingPayment,
    Completed,
    Cancelled,
    Rejected,
}