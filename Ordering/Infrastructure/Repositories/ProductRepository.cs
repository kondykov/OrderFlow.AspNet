using Microsoft.EntityFrameworkCore;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Infrastructure.Repositories;

public class ProductRepository(DataContext context) : IProductRepository
{
    public async Task<int> AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        return await context.SaveChangesAsync();
    }

    public async Task<Product?> FindByIdAsync(int id)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await FindByIdAsync(id);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        return product;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task<List<Product>> GetActivesAsync()
    {
        return await context.Products.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<List<Product>> FindByCategoryAsync(string category)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Product>> FindByNameAsync(string name)
    {
        return await context.Products.Where(p => p.Name == name).ToListAsync();
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var productToUpdate = context.Products.FirstOrDefault(p => p.Id == product.Id);
        if (productToUpdate != null) throw new EntityNotFoundException("Продукт не найден");
        context.Entry<Product>(productToUpdate).CurrentValues.SetValues(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        if (!await context.Products.AnyAsync(p => p.Id == product.Id)) throw new EntityNotFoundException("Продукт не найден");
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}