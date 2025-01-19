using System.Security.Claims;

namespace ECommerceAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public static int GetAuthenticatedUserId(this HttpContext httpContext)
        {
            var userIdClaim = httpContext.User?.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            return int.Parse(userIdClaim.Value);
        }
    }
}
