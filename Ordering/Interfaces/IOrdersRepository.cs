using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrdersRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);

    Task<Order?> FindByIdAsync(int id);
    Task<Order> GetByIdAsync(int id);
    Task<PaginationResponse<List<Order>>> GetAllAsync(int? pageNumber = 1, int? pageSize = 20);
}