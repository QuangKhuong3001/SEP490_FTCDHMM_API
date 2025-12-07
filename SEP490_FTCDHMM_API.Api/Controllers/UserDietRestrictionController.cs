using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserDietRestrictionController : ControllerBase
    {
        private readonly IUserDietRestrictionService _userDietRestrictionService;
        private readonly IMapper _mapper;

        public UserDietRestrictionController(IUserDietRestrictionService userDietRestrictionService, IMapper mapper)
        {
            _userDietRestrictionService = userDietRestrictionService;
            _mapper = mapper;
        }

        [HttpPost("ingredient")]
        public async Task<IActionResult> CreateIngredientRestriction([FromBody] CreateIngredientRestrictionRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserDietRestriction.CreateIngredientRestrictionRequest>(request);

            await _userDietRestrictionService.CreateIngredientRestriction(userId, appRequest);
            return Ok();
        }

        [HttpPost("ingredient-category")]
        public async Task<IActionResult> CreateIngredientCategoryRestriction([FromBody] CreateIngredientCategoryRestrictionRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserDietRestriction.CreateIngredientCategoryRestrictionRequest>(request);

            await _userDietRestrictionService.CreateIngredientCategoryRestriction(userId, appRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserDietRestrictionFilterRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appRequest = _mapper.Map<ApplicationDtos.UserDietRestriction.UserDietRestrictionFilterRequest>(request);

            var result = await _userDietRestrictionService.GetUserDietRestrictionsAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpDelete("{restrictionId:guid}")]
        public async Task<IActionResult> Delete(Guid restrictionId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            await _userDietRestrictionService.DeleteRestriction(userId, restrictionId);
            return Ok();
        }
    }
}
