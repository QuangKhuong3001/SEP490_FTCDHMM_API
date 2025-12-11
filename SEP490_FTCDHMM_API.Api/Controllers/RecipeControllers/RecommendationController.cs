using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommentdationService _recommentdationService;
        private readonly IMapper _mapper;

        public RecommendationController(IRecommentdationService recommentdationService, IMapper mapper)
        {
            _recommentdationService = recommentdationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] PaginationParams request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var result = await _recommentdationService.RecommendRecipesAsync(userId, appRequest);
            return Ok(result);
        }
    }
}
