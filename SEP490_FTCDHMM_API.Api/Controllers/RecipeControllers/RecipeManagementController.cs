using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers.RecipeControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeManagementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeManagementService _recipeManagementService;

        public RecipeManagementController(IMapper mapper, IRecipeManagementService recipeManagementService)
        {
            _mapper = mapper;
            _recipeManagementService = recipeManagementService;
        }

        [Authorize(Policy = PermissionPolicies.Recipe_Lock)]
        [HttpPost("{recipeId:guid}/lock")]
        public async Task<IActionResult> Lock(Guid recipeId, [FromBody] RecipeManagementReasonRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeManagementReasonRequest>(request);

            await _recipeManagementService.LockRecipeAsync(userId, recipeId, appRequest);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Recipe_Approve)]
        [HttpPost("{recipeId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid recipeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _recipeManagementService.ApproveRecipeAsync(userId, recipeId);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Recipe_Approve)]
        [HttpPost("{recipeId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid recipeId, [FromBody] RecipeManagementReasonRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeManagementReasonRequest>(request);

            await _recipeManagementService.RejectRecipeAsync(userId, recipeId, appRequest);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Recipe_Delete)]
        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> Delete(Guid recipeId, [FromBody] RecipeManagementReasonRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.RecipeDtos.RecipeManagementReasonRequest>(request);

            await _recipeManagementService.DeleteRecipeByManageAsync(userId, recipeId, appRequest);
            return Ok();
        }
    }
}
