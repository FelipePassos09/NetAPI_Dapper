using AutoMapper;
using StockCrud.Api.Entities;

namespace StockCrud.Api.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<Order, OrderGetDto>();
        CreateMap<OrderPostDto, Order>();
        CreateMap<Order, OrderGetDto>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

        
        CreateMap<Customer, CustomerGetDto>();
        CreateMap<CustomerPostDto, Customer>();
        
        CreateMap<Product, ProductGetDto>();
        CreateMap<ProductPostDto, Product>();
        
        CreateMap<Category, CategoryGetDto>();
        CreateMap<CategoryPostDto, Category>();
        
        CreateMap<Supplier, SupplierGetDto>();
        CreateMap<SupplierPostDto, Supplier>();
    }
}