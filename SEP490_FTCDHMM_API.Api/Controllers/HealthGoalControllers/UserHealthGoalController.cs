using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.HealthGoalControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserHealthGoalController : ControllerBase
    {
        private readonly IUserHealthGoalService _userHealthGoalService;
        private readonly IMapper _mapper;

        public UserHealthGoalController(IUserHealthGoalService userHealthGoalService, IMapper mapper)
        {
            _userHealthGoalService = userHealthGoalService;
            _mapper = mapper;
        }

        [HttpPost("{goalId:guid}")]
        public async Task<IActionResult> SetGoal(Guid goalId, UserHealthGoalRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserHealthGoalDtos.UserHealthGoalRequest>(request);

            await _userHealthGoalService.SetGoalAsync(userId, goalId, appRequest);
            return Ok();
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentGoal()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _userHealthGoalService.GetCurrentGoalAsync(userId);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryGoal()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _userHealthGoalService.GetHistoryGoalAsync(userId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromCurrent()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _userHealthGoalService.RemoveFromCurrent(userId);
            return Ok();
        }
    }
}
