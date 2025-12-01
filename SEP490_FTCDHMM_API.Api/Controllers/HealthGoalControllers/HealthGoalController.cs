using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterface;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.HealthGoalControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealthGoalController : ControllerBase
    {
        private readonly IHealthGoalService _healthGoalService;
        private readonly IMapper _mapper;

        public HealthGoalController(IHealthGoalService healthGoalService, IMapper mapper)
        {
            _healthGoalService = healthGoalService;
            _mapper = mapper;
        }

        [Authorize(Policy = PermissionPolicies.HealthGoal_Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateHealthGoalRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.HealthGoalDtos.CreateHealthGoalRequest>(request);

            await _healthGoalService.CreateAsync(appRequest);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.HealthGoal_Update)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateHealthGoalRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.HealthGoalDtos.UpdateHealthGoalRequest>(request);

            await _healthGoalService.UpdateAsync(id, appRequest);
            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _healthGoalService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _healthGoalService.GetAllAsync();

            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.HealthGoal_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _healthGoalService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("listGoal")]
        public async Task<IActionResult> GetListGoal()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _healthGoalService.GetListGoalAsync(userId);
            return Ok(result);
        }
    }
}
