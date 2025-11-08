using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserHealthMetricController : ControllerBase
    {
        private readonly IUserHealthMetricService _userHealthMetricService;
        private readonly IMapper _mapper;

        public UserHealthMetricController(IUserHealthMetricService userHealthMetricService, IMapper mapper)
        {
            _userHealthMetricService = userHealthMetricService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserHealthMetricRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserHealthMetricDtos.CreateUserHealthMetricRequest>(request);

            await _userHealthMetricService.CreateAsync(userId, appRequest);
            return Ok();
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid metricId, UpdateUserHealthMetricRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserHealthMetricDtos.UpdateUserHealthMetricRequest>(request);

            await _userHealthMetricService.UpdateAsync(userId, metricId, appRequest);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid metricId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _userHealthMetricService.DeleteAsync(userId, metricId);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _userHealthMetricService.GetHistoryByUserIdAsync(userId);
            var mappedResult = _mapper.Map<IEnumerable<UserHealthMetricResponse>>(result);
            return Ok(mappedResult);
        }

    }
}
