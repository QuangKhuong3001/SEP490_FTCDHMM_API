using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Api.Middleware
{
    public class UserLockoutMiddleware
    {
        private readonly RequestDelegate _next;

        public UserLockoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out var uid))
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user != null && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                {
                    throw new AppException(AppResponseCode.FORBIDDEN, "Tài khoản của bạn đã bị khóa.");
                }
            }

            await _next(context);
        }
    }

}
