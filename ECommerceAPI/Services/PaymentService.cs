using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Repositories;
using Stripe;
using Stripe.Checkout;

namespace ECommerceAPI.Services
{
    public class PaymentService
    {
        private readonly string _secretKey;
        private readonly ICartRepository _cartRepository;

        public PaymentService(IConfiguration configuration, ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _secretKey = configuration["Stripe:SecretKey"]!;
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<Result<string>> CreateCheckoutSession(int userId, string shippingAddress, string currency = "gel")
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart.CartItems.Count == 0)
            {
                return Result<string>.Failure("Cart is empty", ErrorType.BadRequest);
            }

            if (cart.CartItems.Any(ci => ci.Quantity > ci.Product.Stock))
            {
                return Result<string>.Failure("One or more items in your cart are out of stock.", ErrorType.BadRequest);
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.CartItems.Select(ci => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = ci.Product.Name
                        },
                        UnitAmount = (long)(ci.Product.Price * 100)
                    },
                    Quantity = ci.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
                Metadata = new Dictionary<string, string> { { "userId", userId.ToString() }, { "shippingAddress", shippingAddress } }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return Result<string>.SuccessResult(session.Url);
        }
    }
}
