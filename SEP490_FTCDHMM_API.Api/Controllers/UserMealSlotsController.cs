using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [ApiController]
    [Route("api/user-meals")]
    [Authorize]
    public class UserMealSlotsController : ControllerBase
    {
        private readonly IUserMealSlotService _service;
        private readonly IMapper _mapper;

        public UserMealSlotsController(IUserMealSlotService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            return Ok(await _service.GetMyMealsAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MealSlotRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.MealDtos.MealSlotRequest>(request);

            await _service.CreateAsync(userId, appRequest);
            return Ok();
        }

        [HttpPut("{slotId:guid}")]
        public async Task<IActionResult> Update(Guid slotId, [FromBody] MealSlotRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.MealDtos.MealSlotRequest>(request);
            await _service.UpdateAsync(userId, slotId, appRequest);
            return Ok();
        }

        [HttpDelete("{slotId:guid}")]
        public async Task<IActionResult> Delete(Guid slotId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _service.DeleteAsync(userId, slotId);
            return Ok();
        }
    }
}
