using Microsoft.EntityFrameworkCore;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Payments;
using Payments.Interfaces;

namespace Payments.Repositories;

public class PaymentRepository(DataContext context) : IPaymentRepository
{
    public async Task<Payment> Create(Payment payment)
    {
        await context.Payments.AddAsync(payment);
        await context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> Done(Payment payment)
    {
        var existsPayment = await FindById(payment.Id);
        if (existsPayment == null) throw new EntityNotFoundException("Платёж не найден");

        if (existsPayment.Status != PaymentStatus.Awaiting)
            throw new AccessDeniedException($"Платёж можно изменить со статусом \"{PaymentStatus.Awaiting.GetDescription()}\"");
        existsPayment.Status = PaymentStatus.Done;
        await context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment?> FindById(int id)
    {
        return await context.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment?> FindByOrderId(int orderId)
    {
        return await context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task GetAll(PageQuery? query)
    {
        throw new NotImplementedException();
    }
}