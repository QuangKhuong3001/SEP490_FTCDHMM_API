using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;


namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeCommandService _commandService;
        private readonly IRecipeQueryService _queryService;

        public RecipeController(
            IMapper mapper,
            IRecipeCommandService commandService,
            IRecipeQueryService queryService)
        {
            _mapper = mapper;
            _commandService = commandService;
            _queryService = queryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.CreateRecipeRequest>(request);
            var recipeId = await _commandService.CreateRecipeAsync(userId, appRequest);
            return Ok(new { recipeId });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes([FromQuery] RecipeFilterRequest filter)
        {
            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeFilterRequest>(filter);

            var result = await _queryService.GetAllRecipesAsync(appFilter);
            return Ok(result);
        }

        [HttpPut("{recipeId:guid}")]
        public async Task<IActionResult> UpdateRecipe(Guid recipeId, UpdateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UpdateRecipeRequest>(request);
            await _commandService.UpdateRecipeAsync(userId, recipeId, appRequest);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}")]
        public async Task<IActionResult> GetRecipeDetail(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _queryService.GetRecipeDetailsAsync(userId, recipeId);
            return Ok(result);
        }

        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> DeleteRecipe(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _commandService.DeleteRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpGet("favoriteList")]
        public async Task<IActionResult> GetFavoriteList([FromQuery] FavoriteRecipeFilterRequest filter)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.UserFavoriteRecipe.FavoriteRecipeFilterRequest>(filter);
            var result = await _queryService.GetFavoriteListAsync(userId, appFilter);
            return Ok(result);
        }

        [HttpPost("{recipeId:guid}/favorite")]
        public async Task<IActionResult> AddToFavoriteList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _commandService.AddToFavoriteAsync(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}/favorite")]
        public async Task<IActionResult> RemoveFromFavoriteList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _commandService.RemoveFromFavoriteAsync(userId, recipeId);
            return Ok();
        }

        [HttpGet("saveList")]
        public async Task<IActionResult> GetSaveList([FromQuery] SaveRecipeFilterRequest filter)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.UserSaveRecipe.SaveRecipeFilterRequest>(filter);
            var result = await _queryService.GetSavedListAsync(userId, appFilter);
            return Ok(result);
        }

        [HttpPost("{recipeId:guid}/save")]
        public async Task<IActionResult> AddToSaveList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _commandService.SaveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}/save")]
        public async Task<IActionResult> RemoveFromSaveList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _commandService.UnsaveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpGet("myRecipe")]
        public async Task<IActionResult> GetMyRecipeList([FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _queryService.GetRecipeByUserIdAsync(userId, appRequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetRecipesByUserName(string userName, [FromQuery] RecipePaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _queryService.GetRecipeByUserNameAsync(userName, appRequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}/score")]
        public async Task<IActionResult> GetAverageScore(Guid recipeId)
        {
            var avg = await _queryService.GetRecipeRatingAsync(recipeId);
            return Ok(avg);
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}/rating")]
        public async Task<IActionResult> GetByRecipe(Guid recipeId, [FromQuery] RecipePaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var result = await _queryService.GetRatingDetailsAsync(recipeId, appRequest);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetUserHistory([FromQuery] RecipePaginationParams request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var history = await _queryService.GetHistoryAsync(userId, appRequest);
            return Ok(history);
        }

    }
}
