using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("myNotifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost("{notificationId:guid}/mark-read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _notificationService.MarkAsReadAsync(userId, notificationId);
            return Ok();
        }


    }
}