using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

public class UserAccountStatusMiddleware
{
    private readonly RequestDelegate _next;

    public UserAccountStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAuthorizeData>() == null)
        {
            await _next(context);
            return;
        }

        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var user = await userManager.FindByIdAsync(userIdClaim);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, "Tài khoản không tồn tại.");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                throw new AppException(AppResponseCode.FORBIDDEN, "Tài khoản của bạn đã bị khóa.");
        }

        await _next(context);
    }
}
