using AutoMapper;
using OrderFlow.Ordering.Interfaces;
using OrderFlow.Ordering.Models.Requests;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Ordering.Services;

public class ProductService(IProductRepository productRepository, IOrderItemsRepository orderItemsRepository, IMapper mapper)
    : IProductService
{
    public async Task<ProductDto> AddAsync(AddProductRequest request)
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
        return mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (request.Price <= 0) throw new ArgumentException("Цена не может быть меньше 0");
        return mapper.Map<ProductDto>(await productRepository.UpdateAsync(product));
    }

    public async Task<List<ProductDto>> GetAllAsync(int? page = 1, int? pageSize = 20)
    {
        return mapper.Map<List<ProductDto>>(await productRepository.GetAllAsync());
    }

    public async Task<List<Product>> GetAllActiveAsync()
    {
        return mapper.Map<List<Product>>(await productRepository.GetActivesAsync());
    }

    public async Task<ProductDto> FindByIdAsync(int id)
    {
        return mapper.Map<ProductDto>(await productRepository.FindByIdAsync(id));
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        return mapper.Map<ProductDto>(product);
    }

    public async Task<List<ProductDto>> GetByNameAsync(string name)
    {
        return mapper.Map<List<ProductDto>>(await productRepository.GetAllAsync());
    }

    public async Task<bool> DeleteAsync(RemoveProductRequest request)
    {
        var product = await productRepository.FindByIdAsync(request.ProductId);
        if (product == null) throw new EntityNotFoundException("Продукт не найден");
        if (await orderItemsRepository.CheckProductIsUsedAsync(product.Id))
            throw new AccessDeniedException(
                "Нельзя удалять продукт, который уже используется в заказах. Если необходимо вывести продукт из продажи, то установите продукту статус \"Неактивен\"");
        await productRepository.DeleteAsync(product);
        return true;
    }
}