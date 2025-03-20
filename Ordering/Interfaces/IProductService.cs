using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Interfaces;

public interface IProductService
{
    Task<ProductDto> AddAsync(AddProductRequest product);
    Task<ProductDto> UpdateAsync(UpdateProductRequest request);
    Task<List<ProductDto>> GetAllAsync(int? page = 1, int? pageSize = 20);
    Task<List<Product>> GetAllActiveAsync();
    Task<ProductDto> FindByIdAsync(int id);
    Task<ProductDto> GetByIdAsync(int id);
    Task<List<ProductDto>> GetByNameAsync(string name);
    Task<bool> DeleteAsync(RemoveProductRequest productId);
}