using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Warehouse;

namespace OrderFlow.Stock.Interfaces;

public interface IProductReserveSettingsRepository
{
    Task<ProductReserveSettings?> FindSettingsByProduct(Product product);
    Task Save(ProductReserveSettings settings);
}