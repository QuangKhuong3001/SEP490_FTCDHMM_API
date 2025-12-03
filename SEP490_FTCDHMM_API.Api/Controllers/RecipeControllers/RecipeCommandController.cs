using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [ApiController]
    [Route("api/recipe")]
    [Authorize]
    public class RecipeCommandController : ControllerBase
    {
        private readonly IRecipeCommandService _recipeCommandService;
        private readonly IMapper _mapper;

        public RecipeCommandController(IRecipeCommandService recipeCommandService, IMapper mapper)
        {
            _recipeCommandService = recipeCommandService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.CreateRecipeRequest>(request);

            await _recipeCommandService.CreateRecipeAsync(userId, appRequest);
            return Ok();
        }

        [HttpPut("{recipeId:guid}")]
        public async Task<IActionResult> Update(Guid recipeId, [FromForm] UpdateRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UpdateRecipeRequest>(request);

            await _recipeCommandService.UpdateRecipeAsync(userId, recipeId, appRequest);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> Delete(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeCommandService.DeleteRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpPost("{recipeId:guid}/save")]
        public async Task<IActionResult> Save(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _recipeCommandService.SaveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpDelete("{recipeId:guid}/save")]
        public async Task<IActionResult> Unsave(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeCommandService.UnsaveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [HttpPost("{parentId:guid}/copy")]
        public async Task<IActionResult> Copy(Guid parentId, CopyRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.CopyRecipeRequest>(request);

            await _recipeCommandService.CopyRecipe(userId, parentId, appRequest);
            return Ok();
        }
    }
}
