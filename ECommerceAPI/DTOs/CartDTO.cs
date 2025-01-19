namespace ECommerceAPI.DTOs
{
    public class CartDTO
    {
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
}
