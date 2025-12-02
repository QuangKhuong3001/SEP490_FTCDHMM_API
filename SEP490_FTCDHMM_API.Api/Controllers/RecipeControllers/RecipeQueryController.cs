using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [ApiController]
    [Route("api/recipe")]
    public class RecipeQueryController : ControllerBase
    {
        private readonly IRecipeQueryService _recipeQueryService;
        private readonly IMapper _mapper;

        public RecipeQueryController(IRecipeQueryService recipeQueryService, IMapper mapper)
        {
            _recipeQueryService = recipeQueryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RecipeFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeFilterRequest>(request);

            var result = await _recipeQueryService.GetAllRecipesAsync(appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId}")]
        public async Task<IActionResult> GetDetails(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _recipeQueryService.GetRecipeDetailsAsync(userId, recipeId);
            return Ok(result);
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites([FromQuery] FavoriteRecipeFilterRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UserFavoriteRecipe.FavoriteRecipeFilterRequest>(request);

            var result = await _recipeQueryService.GetFavoriteListAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("saved")]
        [Authorize]
        public async Task<IActionResult> GetSaved([FromQuery] SaveRecipeFilterRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UserSaveRecipe.SaveRecipeFilterRequest>(request);

            var result = await _recipeQueryService.GetSavedListAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory([FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var result = await _recipeQueryService.GetHistoryAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetRecipesByUserId(string userName, [FromQuery] RecipePaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _recipeQueryService.GetRecipeByUserNameAsync(userName, appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId:guid}/rating")]
        public async Task<IActionResult> GetByRecipe(Guid recipeId, [FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var result = await _recipeQueryService.GetRatingDetailsAsync(userId, recipeId, appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId:guid}/score")]
        public async Task<IActionResult> GetAverageScore(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var avg = await _recipeQueryService.GetRecipeRatingAsync(userId, recipeId);
            return Ok(avg);
        }

        [Authorize]
        [HttpGet("myRecipe")]
        public async Task<IActionResult> GetMyRecipeList([FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _recipeQueryService.GetRecipeByUserIdAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Policy = PermissionPolicies.Recipe_ManagementView)]
        public async Task<IActionResult> GetPendingList([FromQuery] PaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);
            var result = await _recipeQueryService.GetPendingListAsync(appRequest);
            return Ok(result);
        }

    }
}
