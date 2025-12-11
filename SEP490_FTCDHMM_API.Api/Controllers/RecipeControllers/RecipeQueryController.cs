using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
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

            var result = await _recipeQueryService.GetRecipesAsync(appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId}")]
        public async Task<IActionResult> GetDetails(Guid recipeId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid? userId = null;

            if (Guid.TryParse(userIdClaim, out var parsed))
                userId = parsed;

            var result = await _recipeQueryService.GetRecipeDetailsAsync(userId, recipeId);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.Recipe_ManagementView)]
        [HttpGet("pending/{recipeId}")]
        public async Task<IActionResult> GetDetailsByPermission(Guid recipeId)
        {
            var result = await _recipeQueryService.GetRecipeDetailsByPermissionAsync(recipeId);
            return Ok(result);
        }

        [HttpGet("saved")]
        [Authorize]
        public async Task<IActionResult> GetSaved([FromQuery] SaveRecipeFilterRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.UserSaveRecipe.SaveRecipeFilterRequest>(request);

            var result = await _recipeQueryService.GetSavedRecipesAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory([FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var result = await _recipeQueryService.GetRecipeHistoriesAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetRecipesByUserId(string userName, [FromQuery] RecipePaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _recipeQueryService.GetRecipesByUserNameAsync(userName, appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId:guid}/rating")]
        public async Task<IActionResult> GetByRecipe(Guid recipeId, [FromQuery] RecipePaginationParams request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid? userId = null;

            if (Guid.TryParse(userIdClaim, out var parsed))
                userId = parsed;

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);

            var result = await _recipeQueryService.GetRecipeRatingDetailsAsync(userId, recipeId, appRequest);
            return Ok(result);
        }

        [HttpGet("{recipeId:guid}/score")]
        public async Task<IActionResult> GetAverageScore(Guid recipeId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid? userId = null;

            if (Guid.TryParse(userIdClaim, out var parsed))
                userId = parsed;

            var avg = await _recipeQueryService.GetRecipeRatingsAsync(userId, recipeId);
            return Ok(avg);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRecipeList([FromQuery] RecipePaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipePaginationParams>(request);
            var result = await _recipeQueryService.GetRecipesByUserIdAsync(userId, appRequest);
            return Ok(result);
        }

        [HttpGet("pendingsManagement")]
        [Authorize(Policy = PermissionPolicies.Recipe_ManagementView)]
        public async Task<IActionResult> GetPendingManagementList([FromQuery] PaginationParams request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);
            var result = await _recipeQueryService.GetRecipePendingsAsync(appRequest);
            return Ok(result);
        }

        [HttpGet("pendings")]
        [Authorize]
        public async Task<IActionResult> GetPendingList([FromQuery] PaginationParams request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.Common.PaginationParams>(request);
            var result = await _recipeQueryService.GetRecipePendingsByUserIdAsync(userId, appRequest);
            return Ok(result);
        }
    }
}
