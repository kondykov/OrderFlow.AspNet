using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Payments.Interfaces;
using Payments.Repositories;
using Payments.Services;

namespace Payments;

public static class PaymentsModule
{
    public static void AddPaymentsModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
    }
}