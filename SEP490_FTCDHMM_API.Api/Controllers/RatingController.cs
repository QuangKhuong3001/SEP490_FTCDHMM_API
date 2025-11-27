using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IMapper _mapper;

        public RatingController(IRatingService ratingService, IMapper mapper)
        {
            _ratingService = ratingService;
            _mapper = mapper;
        }

        [HttpPost("{recipeId:guid}")]
        public async Task<IActionResult> Rate(Guid recipeId, [FromBody] RatingRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RatingDtos.RatingRequest>(request);

            await _ratingService.AddOrUpdate(userId, recipeId, appRequest);
            return Ok(new
            {
                Message = "Đánh giá thành công"
            });
        }

        [HttpDelete("{ratingId:guid}")]
        public async Task<IActionResult> Delete(Guid ratingId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _ratingService.Delete(userId, ratingId);
            return Ok(new
            {
                Message = "Xoá đánh giá thành công"
            });
        }
    }
}
