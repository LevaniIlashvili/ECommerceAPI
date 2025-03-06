using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("checkout")]
        public async Task < ActionResult> CreateCheckoutSession([FromBody] CheckoutDTO checkoutDTO)
        {
            var userId = HttpContext.GetAuthenticatedUserId();
            var result = await _paymentService.CreateCheckoutSession(userId, checkoutDTO.ShippingAddress, "gel");

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            };

            return Ok(new { Url = result.Data });
        }
    }
}
