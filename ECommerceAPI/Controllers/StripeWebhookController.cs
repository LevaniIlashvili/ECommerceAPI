using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [Route("api/stripe-webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;

        public StripeWebhookController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["Stripe:WebhookSecret"]);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                if (session?.Metadata.TryGetValue("userId", out var userIdStr) == true &&
                    int.TryParse(userIdStr, out var userId) && session?.Metadata.TryGetValue("shippingAddress", out var shippingAddress) == true)
                {
                    await _orderService.CreateOrder(userId, shippingAddress);
                }
            }

            return Ok();
        }
    }
}
