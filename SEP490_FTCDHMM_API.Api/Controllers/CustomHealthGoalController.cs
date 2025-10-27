using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomHealthGoalController : ControllerBase
    {
        private readonly ICustomHealthGoalService _customHealthGoalService;
        private readonly IMapper _mapper;

        public CustomHealthGoalController(ICustomHealthGoalService customHealthGoalService, IMapper mapper)
        {
            _customHealthGoalService = customHealthGoalService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomHealthGoalRequest req)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.CustomHealthGoalDtos.CreateCustomHealthGoalRequest>(req);

            await _customHealthGoalService.CreateAsync(userId, appRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyGoals()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _customHealthGoalService.GetMyGoalsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _customHealthGoalService.GetByIdAsync(userId, id);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomHealthGoalRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.CustomHealthGoalDtos.UpdateCustomHealthGoalRequest>(request);

            await _customHealthGoalService.UpdateAsync(userId, id, appRequest);

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _customHealthGoalService.DeleteAsync(userId, id);
            return Ok();
        }

        [HttpPut("{id:guid}/active")]
        public async Task<IActionResult> Active(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _customHealthGoalService.ActiveAsync(userId, id);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.HealthGoal_Delete)]
        [HttpPut("{id:guid}/de-active")]
        public async Task<IActionResult> DeActive(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _customHealthGoalService.DeActiveAsync(userId, id);
            return Ok();
        }
    }
}

