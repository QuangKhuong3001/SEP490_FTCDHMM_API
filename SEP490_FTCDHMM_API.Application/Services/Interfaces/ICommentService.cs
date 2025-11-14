using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponse> CreateAsync(Guid userId, Guid recipeId, CreateCommentRequest request);
        Task<CommentResponse> UpdateAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentRequest request);
        Task<List<CommentResponse>> GetAllByRecipeAsync(Guid recipeId);
        Task DeleteAsync(Guid userId, Guid commentId, DeleteMode mode);
    }
}
