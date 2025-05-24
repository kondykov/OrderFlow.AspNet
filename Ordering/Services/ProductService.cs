using Microsoft.AspNetCore.Authorization;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Roles;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Services;

public class ProductService(
    IProductRepository productRepository,
    IOrderItemsRepository orderItemsRepository,
    IUserService userService
)
    : IProductService
{
    public async Task<Product> AddAsync(AddProductRequest request)
    {
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsActive = false
        };
        await productRepository.AddAsync(product);
        return product;
    }

    public async Task<Product> UpdateAsync(UpdateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");
        return await productRepository.UpdateAsync(product);
    }

    [Authorize(Roles = nameof(Manager))]
    public async Task<PaginationResponse<List<Product>>> GetAllAsync(
        int? page = 1,
        int? pageSize = 20,
        bool? isActive = null,
        bool? isSellable = null
    )
    {
        return await productRepository.GetAllAsync(page, pageSize, isActive, isSellable);
    }

    public async Task<Product?> FindByIdAsync(int id)
    {
        return await productRepository.FindByIdAsync(id);
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        return product;
    }

    public async Task<bool> DeleteAsync(RemoveProductRequest request)
    {
        var product = await productRepository.FindByIdAsync(request.ProductId);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        if (await orderItemsRepository.CheckProductIsUsedAsync(product.Id) ||
            (await GetUsingAsComponent(product.Id)).Count > 0)
            throw new AccessDeniedException(
                "Нельзя удалять продукт, который уже используется в заказах или является компонентом другого продукта");
        await productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<Product> AddComponent(int productId, int componentId)
    {
        if (productId == componentId)
            throw new ArgumentException("Циклическая зависимость: продукт и компонент являются одной сущностью");
        var product = await productRepository.GetByIdAsync(productId);
        var component = await productRepository.GetByIdAsync(componentId);

        product.Components.Add(component);
        await productRepository.UpdateAsync(product);
        return product;
    }

    public async Task<Product> RemoveComponent(int productId, int componentId)
    {
        var product = await productRepository.GetByIdAsync(productId);
        var component = await productRepository.GetByIdAsync(componentId);

        if (product.Components.All(c => c.Id != componentId)) return product;
        product.Components.Remove(component);
        await productRepository.UpdateAsync(product);
        return product;
    }

    public async Task<List<Product>> GetUsingAsComponent(int productId)
    {
        var product = await productRepository.GetByIdAsync(productId);
        return await productRepository.GetUsingAsComponentAsync(product);
    }
}