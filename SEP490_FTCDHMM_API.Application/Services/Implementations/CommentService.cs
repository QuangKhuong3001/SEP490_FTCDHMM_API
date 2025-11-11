using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _notifier;

        public CommentService(
            ICommentRepository commentRepository,
            IMapper mapper,
            IRealtimeNotifier notifier)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _notifier = notifier;
        }

        public async Task<CommentResponse> CreateAsync(Guid userId, Guid recipeId, CreateCommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            comment.UserId = userId;
            comment.RecipeId = recipeId;
            comment.CreatedAtUtc = DateTime.UtcNow;

            // If replying to a level 2+ comment, reply to its parent instead (limit to max 2 levels)
            if (comment.ParentCommentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(
                    comment.ParentCommentId.Value,
                    c => c.Include(x => x.ParentComment)
                );

                if (parentComment != null && parentComment.ParentCommentId.HasValue)
                {
                    // This is a level 2 comment, so set parent to its parent (level 1)
                    comment.ParentCommentId = parentComment.ParentCommentId;
                }
            }

            await _commentRepository.AddAsync(comment);
            var saved = await _commentRepository.GetByIdAsync(comment.Id, c => c.Include(x => x.User).ThenInclude(x => x.Avatar));

            var response = _mapper.Map<CommentResponse>(saved);

            // gửi realtime tới group của recipe
            await _notifier.SendCommentAddedAsync(recipeId, response);

            return response;
        }

        public async Task<List<CommentResponse>> GetAllByRecipeAsync(Guid recipeId)
        {
            var comments = await _commentRepository.GetAllAsync(
                c => c.RecipeId == recipeId && c.ParentCommentId == null,
                c => c.Include(x => x.User).ThenInclude(x => x.Avatar)
                      .Include(x => x.Replies).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
                      .Include(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.User).ThenInclude(x => x.Avatar)
            );

            return _mapper.Map<List<CommentResponse>>(comments);
        }

        public async Task DeleteAsync(Guid userId, Guid commentId, DeleteMode mode)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId,
                include: i => i.Include(c => c.Recipe)
                                .Include(c => c.Replies));
            if (comment == null)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (mode == DeleteMode.Self && comment.UserId != userId)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (mode == DeleteMode.RecipeAuthor && comment.Recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var recipeId = comment.RecipeId;

            await DeleteRepliesRecursive(comment);

            await _commentRepository.DeleteAsync(comment);

            await _notifier.SendCommentDeletedAsync(recipeId, commentId);
        }

        private async Task DeleteRepliesRecursive(Comment parent)
        {
            if (parent.Replies == null || !parent.Replies.Any())
                return;

            var repliesCopy = parent.Replies.ToList();

            foreach (var reply in repliesCopy)
            {
                var fullReply = await _commentRepository.GetByIdAsync(reply.Id, c => c.Replies);
                if (fullReply != null)
                {
                    await DeleteRepliesRecursive(fullReply);

                    await _commentRepository.DeleteAsync(fullReply);
                }
            }
        }

    }
}
