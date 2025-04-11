using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Payments;

namespace Payments.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> Create(Payment payment);
    Task<Payment> Done(Payment payment);
    Task<Payment?> FindById(int id);
    Task<Payment?> FindByOrderId(int orderId);
    Task GetById(int id);
    Task GetAll(PageQuery? query);
}