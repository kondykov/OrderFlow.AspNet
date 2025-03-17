using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<int> AddAsync(AddProductRequest request)
    {
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");
        
        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsActive = false,
        };
        
        return await repository.AddAsync(product);
    }

    public async Task<Product> UpdateAsync(UpdateProductRequest request)
    {
        var product = await repository.GetByIdAsync(request.Id);
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");
        return await repository.UpdateAsync(product);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<List<Product>> GetAllActiveAsync()
    {
        return await repository.GetActivesAsync();
    }

    public async Task<Product?> FindByIdAsync(int id)
    {
        return await repository.FindByIdAsync(id);
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await FindByIdAsync(id);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        return product;
    }

    public async Task<List<Product>> GetByNameAsync(string name)
    {
        return await repository.FindByNameAsync(name);
    }

    public async Task<bool> DeleteAsync(int productId)
    {
        var product = await repository.FindByIdAsync(productId);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        if (product.OrderItems.Count > 0)
            throw new AccessDeniedException(
                "Нельзя удалять продукт, который уже используется в заказах. Если необходимо вывести продукт из продажи, то установите продукту статус \"Неактивен\"");
        await repository.DeleteAsync(product);
        return true;
    }
}