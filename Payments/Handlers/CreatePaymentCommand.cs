namespace Payments.Handlers;

public class CreatePaymentCommand
{
    public required int OrderId { get; set; }
    public required string PaymentProvider { get; set; }
    public decimal Amount { get; set; }
}