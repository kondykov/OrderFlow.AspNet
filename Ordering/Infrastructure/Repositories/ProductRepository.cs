using Microsoft.EntityFrameworkCore;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models;
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

    public async Task<PaginationResponse<List<Product>>> GetAllAsync(
        int? page = 1,
        int? pageSize = 20,
        bool? isActive = null,
        bool? isSellable = null
    )
    {
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;

        if (currentPage < 1) currentPage = 1;
        if (currentPageSize < 1) currentPageSize = 20;

        var query = context.Products
            .Include(p => p.Components)
            .AsQueryable();

        if (isActive.HasValue) query = query.Where(p => p.IsActive == isActive.Value);
        if (isSellable.HasValue) query = query.Where(p => p.IsSellable == isSellable.Value);

        return new PaginationResponse<List<Product>>
        {
            Pages = 0,
            Data = await query
                .OrderByDescending(p => p.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToListAsync(),
            Page = currentPage,
            PageSize = currentPageSize
        };
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
        if (productToUpdate == null) throw new EntityNotFoundException("Обновляемый продукт не найден");
        context.Entry(productToUpdate).CurrentValues.SetValues(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        if (!await context.Products.AnyAsync(p => p.Id == product.Id))
            throw new EntityNotFoundException("Продукт не найден");
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetUsingAsComponentAsync(Product targetProduct)
    {
        var product = await context.Products
            .Include(p => p.UsedIn)
            .FirstAsync(p => p.Id == targetProduct.Id);
        
        return product.UsedIn;
    }
}