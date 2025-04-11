using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Shared.Models.Payments;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public PaymentStatus Status { get; set; } = PaymentStatus.Awaiting;
    public decimal Amount { get; set; } = 0;
    public required string PaymentProvider { get; set; }
}