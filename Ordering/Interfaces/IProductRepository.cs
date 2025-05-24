using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IProductRepository
{
    Task<int> AddAsync(Product product);
    Task<Product?> FindByIdAsync(int id);
    Task<Product> GetByIdAsync(int id);

    Task<PaginationResponse<List<Product>>> GetAllAsync(
        int? page = 1,
        int? pageSize = 20,
        bool? isActive = null,
        bool? isSellable = null
    );

    Task<List<Product>> FindByCategoryAsync(string category);
    Task<List<Product>> FindByNameAsync(string name);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<List<Product>> GetUsingAsComponentAsync(Product product);
}