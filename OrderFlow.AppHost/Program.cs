using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OrderFlow.AppHost;
using OrderFlow.Identity;
using OrderFlow.Ordering;
using OrderFlow.Shared;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Stock;
using Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default") ?? string.Empty));

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            //NamingStrategy = new SnakeCaseNamingStrategy()
        };
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    });

builder.AddIdentityModule();
builder.AddOrderingModule();
builder.AddSwaggerModule();
builder.AddSharedModule();
builder.AddPaymentsModule();
builder.AddWarehouseModule();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Входящий запрос: {context.Request.Method} {context.Request.Path}");
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        if (app.Environment.IsDevelopment())
            logger.LogError($"Ошибка: {context.Request.Method} {context.Request.Path}: {ex.Message}");
    }
    finally
    {
        logger.LogInformation(
            $"Запрос выполнен: {context.Request.Method} {context.Request.Path} [{context.Response.StatusCode}] ");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();