using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IProductService
{
    Task<int> AddAsync(AddProductRequest product);
    Task<Product> UpdateAsync(UpdateProductRequest request);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetAllActiveAsync();
    Task<Product?> FindByIdAsync(int id);
    Task<Product> GetByIdAsync(int id);
    Task<List<Product>> GetByNameAsync(string name);
    Task<bool> DeleteAsync(int productId);
}