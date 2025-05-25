using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Models.Warehouse;
using OrderFlow.Stock.Interfaces;

namespace OrderFlow.Stock.Services;

public class WarehouseService(
    IProductReserveSettingsRepository productReserveSettingsRepository,
    IWarehouseRepository repository,
    IProductService productService
) : IWarehouseService
{
    public async Task<ProductReserve> AddProductReserveAsync(int productId, double quantity)
    {
        if (quantity < 0) throw new ArgumentException("Количество не может быть отрицательным");
        var product = await productService.GetByIdAsync(productId);
        var productReserve = await repository.AddProductReserveAsync(product, quantity);
        var productReserveSettings = new ProductReserveSettings
        {
            ProductId = product.Id,
        };
        await productReserveSettingsRepository.Save(productReserveSettings);
        return productReserve;
    }

    public async Task<ProductReserve> TakeProductReserveAsync(int productId, double quantity)
    {
        var product = await productService.GetByIdAsync(productId);
        var reserves = await GetProductReserveAsync(productId);
        // var settings = await productReserveSettingsRepository.FindSettingsByProduct(product);
        if (reserves.Quantity < quantity /*&& !settings!.IgnoreOutOfStock*/)
            throw new ArgumentException("Нельзя взять со склада больше, чем есть на складе");
        if (quantity < 0) throw new ArgumentException("Количество не может быть отрицательным");
        return await repository.TakeProductReserveAsync(product, quantity);
    }

    public async Task<ProductReserve?> FindProductReserveAsync(int productId)
    {
        var product = await productService.GetByIdAsync(productId);
        return await repository.FindProductReserveAsync(product);
    }

    public async Task<ProductReserve> GetProductReserveAsync(int productId)
    {
        var product = await productService.GetByIdAsync(productId);
        return await repository.GetProductReserveAsync(product);
    }
}