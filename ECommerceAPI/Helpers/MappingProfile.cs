using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Cart, CartDTO>();
            CreateMap<CartItem, CartItemDTO>();

            CreateMap<Order, OrderDTO>();
            CreateMap<OrderItem, OrderItemDTO>();
        }
    }
}
