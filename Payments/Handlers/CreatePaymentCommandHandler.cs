using Microsoft.Extensions.Logging;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Extensions;
using OrderFlow.Shared.Models.Payments;
using Payments.Interfaces;

namespace Payments.Handlers;

public class CreatePaymentCommandHandler(
    IPaymentRepository repository,
    IOrderService orderService,
    ILogger<CreatePaymentCommandHandler> logger
    ) : ICommandHandler<CreatePaymentCommand, Payment>
{
    public async Task<Payment> Handle(CreatePaymentCommand command)
    {
        // todo: реализовать подсчёт
        var order = await orderService.GetAsync(command.OrderId);
        var amount = order?.OrderItems.Sum(item => item.Product.Price * item.Quantity);

        if (amount is < 0 or null)
        {
            logger.LogError($"Ошибка при подсчёте суммы заказа {order?.Id}: сумма заказа {amount ?? null}");
            throw new Exception("Внутренняя ошибка при подсчёте суммы заказа");
        }
        
        var payment = new Payment()
        {
            OrderId = order.Id,
            Order = order,
            Amount = amount ?? 0,
            PaymentProvider = command.PaymentProvider,
        };
        
        return await repository.Create(payment);
    }
}