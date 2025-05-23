using AutoMapper;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Services;

public class ProductService(IProductRepository productRepository, IOrderItemsRepository orderItemsRepository, IUserService userService, IMapper mapper)
    : IProductService
{
    public async Task<Product> AddAsync(AddProductRequest request)
    {
        await userService.RequireClaimAsync(SystemClaims.CanCreateProduct);
        
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
        await userService.RequireClaimAsync(SystemClaims.CanEditProduct);

        var product = await productRepository.GetByIdAsync(request.Id);
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");
        return await productRepository.UpdateAsync(product);
    }

    public async Task<PaginationResponse<List<Product>>> GetAllAsync(int? page = 1, int? pageSize = 20, bool? isActive = null, bool? isSellable = null)
    {
        return await productRepository.GetAllAsync(page, pageSize, isActive, isSellable);
    }

    public async Task<List<Product>> GetAllActiveAsync()
    {
        return await productRepository.GetActivesAsync();
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
        await userService.RequireClaimAsync(SystemClaims.CanDeleteProduct);
        
        var product = await productRepository.FindByIdAsync(request.ProductId);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        if (await orderItemsRepository.CheckProductIsUsedAsync(product.Id))
            throw new AccessDeniedException(
                "Нельзя удалять продукт, который уже используется в заказах. Если необходимо вывести продукт из продажи, то установите продукту статус \"Неактивен\"");
        await productRepository.DeleteAsync(product);
        return true;
    }
}