using Microsoft.EntityFrameworkCore;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Warehouse;
using OrderFlow.Stock.Interfaces;

namespace OrderFlow.Stock.Repositories;

public class WarehouseRepository(DataContext context) : IWarehouseRepository
{
    public async Task<ProductReserve> AddProductReserveAsync(Product product, double quantity)
    {
        var reserves = await FindProductReserveAsync(product);
        if (reserves == null)
        {
            reserves = new ProductReserve
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity
            };
            context.ProductReserves.Add(reserves);
            await context.SaveChangesAsync();
            return reserves;
        }
        reserves.Quantity += quantity;
        context.ProductReserves.Update(reserves);
        await context.SaveChangesAsync();
        return reserves;
    }

    public async Task<ProductReserve> TakeProductReserveAsync(Product product, double quantity)
    {
        var reserves = await GetProductReserveAsync(product);
        reserves.Quantity -= quantity;
        context.ProductReserves.Update(reserves);
        await context.SaveChangesAsync();
        return reserves;
    }

    public async Task<ProductReserve?> FindProductReserveAsync(Product product)
    {
        return await context.ProductReserves.Where(pr => pr.ProductId == product.Id).FirstOrDefaultAsync();
    }

    public async Task<ProductReserve> GetProductReserveAsync(Product product)
    {
        var reserves = await FindProductReserveAsync(product);
        if (reserves == null) throw new EntityNotFoundException($"Продукт (id:{product.Id})\"{product.Name}\" не зарегистрирован на складе");
        return reserves;
    }
}