using OrderFlow.Shared.Models.Ordering;
using Payments.Models.Response;

namespace Payments.Interfaces;

public interface IPaymentService
{
    Task<FullPaymentAmountResponse> GetPaymentDetailsAsync(Order order);
    Task Create(Order order, decimal amount);
    Task Process(long paymentId);
    Task Cancel(long paymentId);
    Task Refund(long paymentId);
    Task Get(long paymentId);
    
    /*
    Task<Payments> CreatePaymentAsync(Order order, decimal amount);
    Task<Payments> ProcessPaymentAsync(string paymentId);
    Task CancelPaymentAsync(string paymentId);
    Task RefundPaymentAsync(string paymentId);
    Task<Payments> GetPaymentAsync(string paymentId);
    */
}