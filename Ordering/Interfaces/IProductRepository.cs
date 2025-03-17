using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IProductRepository
{
    Task<int> AddAsync(Product product);
    Task<Product?> FindByIdAsync(int id);
    Task<Product> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetActivesAsync();
    Task<List<Product>> FindByCategoryAsync(string category);
    Task<List<Product>> FindByNameAsync(string name);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}