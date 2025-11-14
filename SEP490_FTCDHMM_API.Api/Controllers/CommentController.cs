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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("{recipeId:guid}")]
        public async Task<IActionResult> GetAll(Guid recipeId)
        {
            var result = await _commentService.GetAllByRecipeAsync(recipeId);
            return Ok(new { message = "Lấy danh sách bình luận thành công", data = result });
        }

        [HttpPost("{recipeId:guid}")]
        public async Task<IActionResult> Create(Guid recipeId, [FromBody] CreateCommentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.CommentDtos.CreateCommentRequest>(request);

            var response = await _commentService.CreateAsync(userId, recipeId, appRequest);
            return Ok(new { message = "Tạo bình luận thành công", data = response });
        }

        [HttpPut("{recipeId:guid}/{commentId:guid}")]
        public async Task<IActionResult> Update(Guid recipeId, Guid commentId, [FromBody] UpdateCommentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appRequest = _mapper.Map<ApplicationDtos.CommentDtos.UpdateCommentRequest>(request);

            var response = await _commentService.UpdateAsync(userId, recipeId, commentId, appRequest);
            return Ok(new { message = "Cập nhật bình luận thành công", data = response });
        }


        [HttpDelete("{commentId:guid}")]
        public async Task<IActionResult> DeleteOwn(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.Self);
            return Ok(new { message = "Bình luận đã được xóa thành công" });
        }

        [HttpDelete("{commentId:guid}/by-author")]
        public async Task<IActionResult> DeleteByRecipeAuthor(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.RecipeAuthor);
            return Ok(new { message = "Bình luận đã được xóa bởi tác giả công thức" });
        }

        [Authorize(Policy = PermissionPolicies.Comment_Delete)]
        [HttpDelete("{commentId:guid}/manage")]
        public async Task<IActionResult> DeleteWithPermission(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.DeleteAsync(userId, commentId, DeleteMode.Permission);
            return Ok(new { message = "Bình luận đã được xóa với quyền quản lý" });
        }

    }
}
