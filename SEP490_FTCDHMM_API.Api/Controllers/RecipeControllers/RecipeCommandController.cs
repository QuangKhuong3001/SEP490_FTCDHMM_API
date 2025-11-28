using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    [Authorize]
    public class RecipeCommandController : ControllerBase
    {
        private readonly IRecipeCommandService _recipeCommandService;

        public RecipeCommandController(IRecipeCommandService recipeCommandService)
        {
            _recipeCommandService = recipeCommandService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateRecipeRequest request)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            var recipeId = await _recipeCommandService.CreateRecipeAsync(userId, request);
            return Ok(new { RecipeId = recipeId });
        }

        [HttpPut("{recipeId}")]
        public async Task<IActionResult> Update(Guid recipeId, [FromForm] UpdateRecipeRequest request)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.UpdateRecipeAsync(userId, recipeId, request);
            return Ok();
        }

        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> Delete(Guid recipeId)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.DeleteRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpPost("{recipeId}/favorite")]
        public async Task<IActionResult> AddToFavorite(Guid recipeId)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.AddToFavoriteAsync(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId}/favorite")]
        public async Task<IActionResult> RemoveFromFavorite(Guid recipeId)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.RemoveFromFavoriteAsync(userId, recipeId);
            return Ok();
        }


        [HttpPost("{recipeId}/save")]
        public async Task<IActionResult> Save(Guid recipeId)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.SaveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId}/save")]
        public async Task<IActionResult> Unsave(Guid recipeId)
        {
            var userId = Guid.Parse(User.Identity!.Name!);
            await _recipeCommandService.UnsaveRecipeAsync(userId, recipeId);
            return Ok();
        }
    }
}
