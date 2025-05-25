using Microsoft.EntityFrameworkCore;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Warehouse;
using OrderFlow.Stock.Interfaces;

namespace OrderFlow.Stock.Repositories;

public class ProductReserveSettingsRepository(DataContext context) : IProductReserveSettingsRepository
{
    public async Task<ProductReserveSettings?> FindSettingsByProduct(Product product)
    {
        var settings = context.ProductReserveSettings.FirstOrDefault(p => p.ProductId == product.Id);
        // if (settings == null) throw new EntityNotFoundException("Настройки не найдены");
        return settings;
    }

    public async Task Save(ProductReserveSettings settings)
    {
        var s = await context.ProductReserveSettings.FirstOrDefaultAsync(prs => prs.ProductId == settings.ProductId);
        if (s != null) await context.ProductReserveSettings.AddAsync(settings);
        else context.ProductReserveSettings.Update(settings);
        await context.SaveChangesAsync();
    }
}