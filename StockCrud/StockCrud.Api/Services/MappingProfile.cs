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
        
        
        CreateMap<Entities.Customer, CustomerGetDto>();
        CreateMap<Entities.Customer, CustomerSearchDto>();
        CreateMap<CustomerPostDto, Entities.Customer>();
        
        CreateMap<Entities.Product, ProductGetDto>();
        CreateMap<ProductPostDto, Entities.Product>();
        
        CreateMap<Category, CategoryGetDto>();
        CreateMap<CategoryPostDto, Category>();
        
        CreateMap<Supplier, SupplierGetDto>();
        CreateMap<SupplierPostDto, Supplier>();
    }
}