using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet("myNotifications")]
        public async Task<IActionResult> GetMyNotifications([FromQuery] PaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);

            var result = await _notificationService.GetNotificationsByUserIdAsync(userId, appRequest);
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