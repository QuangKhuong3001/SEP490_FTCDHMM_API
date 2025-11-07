using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/recipes/{recipeId:guid}/ratings")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IMapper _mapper;

        public RatingController(IRatingService ratingService, IMapper mapper)
        {
            _ratingService = ratingService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("average")]
        public async Task<IActionResult> GetAverage(Guid recipeId)
        {
            var avg = await _ratingService.GetAverageRatingAsync(recipeId);
            return Ok(new { averageRating = avg });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rate(Guid recipeId, [FromBody] CreateRatingRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RatingDtos.CreateRatingRequest>(request);

            var result = await _ratingService.AddOrUpdateAsync(userId, recipeId, appRequest);
            return Ok(result);
        }
    }
}
