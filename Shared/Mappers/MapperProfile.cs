using AutoMapper;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Ordering.DTOs;

namespace OrderFlow.Shared.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderDto, Order>();

        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItemDto, OrderItem>();

        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
        
        CreateMap<User, UserDto>();
    }
}