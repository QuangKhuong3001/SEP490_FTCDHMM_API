using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task CreateCommentAsync(Guid userId, Guid recipeId, CreateCommentRequest request);
        Task UpdateCommentAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentRequest request);
        Task<List<CommentResponse>> GetAllCommentByRecipeAsync(Guid recipeId);
        Task DeleteCommentAsync(Guid userId, Guid commentId, DeleteMode mode);
    }
}
