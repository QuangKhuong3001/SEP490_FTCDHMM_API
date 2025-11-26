using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;


namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommentdationService _recommentdationService;

        public RecommendationController(IRecommentdationService recommentdationService)
        {
            _recommentdationService = recommentdationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _recommentdationService.RecommendAsync(userId);
            return Ok(result);
        }
    }
}
