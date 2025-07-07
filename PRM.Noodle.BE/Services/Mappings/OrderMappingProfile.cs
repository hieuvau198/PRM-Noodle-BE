using AutoMapper;
using PRM.Noodle.BE.Share.Models;
using Services.DTOs.Order;
using System.Linq;

namespace Services.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => "pending"))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => "pending"))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.OrderCombos, opt => opt.Ignore());

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.OrderCombos, opt => opt.MapFrom(src => src.OrderCombos));

            CreateMap<Order, OrderListDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => 
                    (src.OrderItems.Sum(oi => oi.Quantity ?? 0) + 
                     src.OrderCombos.Sum(oc => oc.Quantity ?? 0))));

            CreateMap<CreateOrderItemDto, OrderItem>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Subtotal, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItemToppings, opt => opt.Ignore());

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity ?? 0))
                .ForMember(dest => dest.Toppings, opt => opt.MapFrom(src => src.OrderItemToppings));

            CreateMap<CreateOrderComboDto, OrderCombo>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Subtotal, opt => opt.Ignore());

            CreateMap<OrderCombo, OrderComboDto>()
                .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.Combo.ComboName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity ?? 0));

            CreateMap<CreateOrderItemToppingDto, OrderItemTopping>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Subtotal, opt => opt.Ignore());

            CreateMap<OrderItemTopping, OrderItemToppingDto>()
                .ForMember(dest => dest.ToppingName, opt => opt.MapFrom(src => src.Topping.ToppingName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity ?? 0));
        }
    }
}
