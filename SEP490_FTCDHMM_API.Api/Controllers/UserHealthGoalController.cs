using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> SetGoal(Guid goalId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _userHealthGoalService.SetGoalAsync(userId, goalId);
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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveFromCurrent(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _userHealthGoalService.RemoveFromCurrent(userId, id);
            return Ok();
        }
    }
}
