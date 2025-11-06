using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/recipes/{recipeId:guid}/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid recipeId)
        {
            var result = await _commentService.GetAllByRecipeAsync(recipeId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Guid recipeId, [FromBody] CreateCommentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.CommentDtos.CreateCommentRequest>(request);

            var result = await _commentService.CreateAsync(userId, recipeId, appRequest);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{commentId:guid}")]
        public async Task<IActionResult> DeleteOwn(Guid recipeId, Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.Self);
            return Ok(new { message = "Your comment has been deleted." });
        }

        [Authorize]
        [HttpDelete("{commentId:guid}/by-author")]
        public async Task<IActionResult> DeleteByRecipeAuthor(Guid recipeId, Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.RecipeAuthor);
            return Ok(new { message = "Comment deleted by recipe author." });
        }

        [Authorize(Policy = PermissionPolicies.Comment_Delete)]
        [HttpDelete("{commentId:guid}/manage")]
        public async Task<IActionResult> DeleteWithPermission(Guid recipeId, Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.Permission);
            return Ok(new { message = "Comment deleted with elevated permission." });
        }

    }
}
