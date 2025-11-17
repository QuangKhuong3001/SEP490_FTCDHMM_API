using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;


namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeService _recipeService;

        public RecipeController(IMapper mapper, IRecipeService recipeService)
        {
            _mapper = mapper;
            _recipeService = recipeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.CreateRecipeRequest>(request);
            await _recipeService.CreatRecipe(userId, appRequest);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes([FromQuery] RecipeFilterRequest filter)
        {
            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeFilterRequest>(filter);

            var result = await _recipeService.GetAllRecipes(appFilter);
            return Ok(result);
        }

        [HttpPut("{recipeId:guid}")]
        public async Task<IActionResult> UpdateRecipe(Guid recipeId, UpdateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UpdateRecipeRequest>(request);
            await _recipeService.UpdateRecipe(userId, recipeId, appRequest);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}")]
        public async Task<IActionResult> GetRecipeDetail(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _recipeService.GetRecipeDetails(userId, recipeId);
            return Ok(result);
        }

        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> DeleteRecipe(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeService.DeleteRecipe(userId, recipeId);
            return Ok();
        }

        [HttpGet("favoriteList")]
        public async Task<IActionResult> GetFavoriteList([FromQuery] FavoriteRecipeFilterRequest filter)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.UserFavoriteRecipe.FavoriteRecipeFilterRequest>(filter);
            var result = await _recipeService.GetFavoriteList(userId, appFilter);
            return Ok(result);
        }

        [HttpPost("{recipeId:guid}/favorite")]
        public async Task<IActionResult> AddToFavoriteList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeService.AddToFavorite(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}/favorite")]
        public async Task<IActionResult> RemoveFromFavoriteList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeService.RemoveFromFavorite(userId, recipeId);
            return Ok();
        }

        [HttpGet("saveList")]
        public async Task<IActionResult> GetSaveList([FromQuery] SaveRecipeFilterRequest filter)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appFilter = _mapper.Map<ApplicationDtos.RecipeDtos.UserSaveRecipe.SaveRecipeFilterRequest>(filter);
            var result = await _recipeService.GetSavedList(userId, appFilter);
            return Ok(result);
        }

        [HttpPost("{recipeId:guid}/save")]
        public async Task<IActionResult> AddToSaveList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeService.SaveRecipe(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}/save")]
        public async Task<IActionResult> RemoveFromSaveList(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeService.UnsaveRecipe(userId, recipeId);
            return Ok();
        }

        [HttpGet("myRecipe")]
        public async Task<IActionResult> GetMyRecipeList([FromQuery] PaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);
            var result = await _recipeService.GetRecipeByUserId(userId, appRequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetRecipesByUserId(Guid userId, [FromQuery] PaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);
            var result = await _recipeService.GetRecipeByUserId(userId, appRequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}/score")]
        public async Task<IActionResult> GetAverageScore(Guid recipeId)
        {
            var avg = await _recipeService.GetAverageScore(recipeId);
            return Ok(new { averageRating = avg });
        }

        [AllowAnonymous]
        [HttpGet("{recipeId:guid}/raiting")]
        public async Task<IActionResult> GetByRecipe(Guid recipeId, [FromQuery] PaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);

            var result = await _recipeService.GetRaiting(recipeId, appRequest);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetUserHistory([FromQuery] PaginationParams request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);

            var history = await _recipeService.GetHistory(userId, appRequest);
            return Ok(history);
        }

    }
}
