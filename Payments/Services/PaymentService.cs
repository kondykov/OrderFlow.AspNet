using OrderFlow.Shared.Models.Ordering;
using Payments.Interfaces;
using Payments.Models.Response;

namespace Payments.Services;

public class PaymentService : IPaymentService
{
    public async Task<FullPaymentAmountResponse> GetPaymentDetailsAsync(Order order)
    {
        var orderDetails = new FullPaymentAmountResponse
        {
            OrderId = order.Id,
            Amount = order.OrderItems?.Sum(item => item.Quantity * item.Product.Price) ?? 0
        };
        return orderDetails;
    }

    public async Task Create(Order order, decimal amount)
    {
        var payments = order.Payments;
        var paymentAmounts = payments?.Sum(payment => payment.Amount) ?? 0;
        var amounts = order.OrderItems?.Sum(item => item.Quantity * item.Product.Price) ?? 0;
        if (paymentAmounts + amount > amounts)
            throw new ArgumentException("Нельзя выставить счёт больше, чем стоимость заказа");

        throw new NotImplementedException();
    }

    public async Task Process(long paymentId)
    {
        throw new NotImplementedException();
    }

    public async Task Cancel(long paymentId)
    {
        throw new NotImplementedException();
    }

    public async Task Refund(long paymentId)
    {
        throw new NotImplementedException();
    }

    public async Task Get(long paymentId)
    {
        throw new NotImplementedException();
    }
}