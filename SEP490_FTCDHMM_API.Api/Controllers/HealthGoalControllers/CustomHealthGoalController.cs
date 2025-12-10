using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.HealthGoalControllers
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

            await _customHealthGoalService.CreateCustomHealthGoalAsync(userId, appRequest);
            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _customHealthGoalService.GetCustomHealthGoalByIdAsync(userId, id);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomHealthGoalRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.CustomHealthGoalDtos.UpdateCustomHealthGoalRequest>(request);

            await _customHealthGoalService.UpdateCustomHealthGoalAsync(userId, id, appRequest);

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _customHealthGoalService.DeleteCustomHealthGoalAsync(userId, id);
            return Ok();
        }
    }
}

