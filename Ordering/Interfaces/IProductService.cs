using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Interfaces;

public interface IProductService
{
    Task<Product> AddAsync(AddProductRequest product);
    Task<Product> UpdateAsync(UpdateProductRequest request);
    Task<PaginationResponse<List<Product>>> GetAllAsync(int? page = 1, int? pageSize = 20, bool? isActive = null, bool? isSellable = null);
    Task<Product?> FindByIdAsync(int id);
    Task<Product> GetByIdAsync(int id);
    Task<bool> DeleteAsync(RemoveProductRequest productId);
}