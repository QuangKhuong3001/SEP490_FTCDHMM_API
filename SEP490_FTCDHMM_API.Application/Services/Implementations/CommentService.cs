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
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly INotificationRepository _notificationRepository;


        public CommentService(
            ICommentRepository commentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            INotificationRepository notificationRepository,
            IRealtimeNotifier notifier)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _notifier = notifier;
        }

        public async Task CreateAsync(Guid userId, Guid recipeId, CreateCommentRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var userExist = await _userRepository.ExistsAsync(u => u.Id == userId);
            if (!userExist)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var recipeExist = await _recipeRepository.ExistsAsync(r => r.Id == recipeId);
            if (!recipeExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var comment = new Comment
            {
                UserId = userId,
                RecipeId = recipeId,
                CreatedAtUtc = DateTime.UtcNow,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId
            };

            if (request.MentionedUserIds.Any())
            {
                foreach (var mentionedId in request.MentionedUserIds.Distinct())
                {
                    if (!await _userRepository.ExistsAsync(u => u.Id == mentionedId))
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION,
                            "Người dùng được nhắc tới không tồn tại.");

                    comment.Mentions.Add(new CommentMention
                    {
                        CommentId = comment.Id,
                        MentionedUserId = mentionedId
                    });
                }
            }

            if (comment.ParentCommentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(
                    comment.ParentCommentId.Value,
                    c => c.Include(x => x.ParentComment)
                );

                if (parentComment != null && parentComment.ParentCommentId.HasValue)
                {
                    comment.ParentCommentId = parentComment.ParentCommentId;
                }

            }

            await _commentRepository.AddAsync(comment);
            var saved = await _commentRepository.GetByIdAsync(
                comment.Id,
                c => c
                    .Include(x => x.User).ThenInclude(x => x.Avatar)
                    .Include(x => x.Mentions).ThenInclude(x => x.MentionedUser)
            );

            var response = _mapper.Map<CommentResponse>(saved);

            await _notifier.SendCommentAddedAsync(recipeId, response);

            if (comment.ParentCommentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(
                    comment.ParentCommentId.Value,
                    c => c.Include(x => x.ParentComment)
                );

                if (parentComment != null)
                {
                    // Notify parent comment author about reply
                    await CreateAndSendNotificationAsync(userId, parentComment.UserId, NotificationType.Reply, null, recipeId);

                    // Notify recipe author about comment
                    await CreateAndSendNotificationAsync(userId, recipe.AuthorId, NotificationType.Comment, null, recipeId);
                }
                else
                {
                    await CreateAndSendNotificationAsync(userId, recipe.AuthorId, NotificationType.Comment, null, recipeId);
                }
            }
            else
            {
                await CreateAndSendNotificationAsync(userId, recipe.AuthorId, NotificationType.Comment, null, recipeId);
            }

        }

        /// <summary>
        /// Helper method to create, save, and send real-time notification via SignalR
        /// </summary>
        private async Task CreateAndSendNotificationAsync(Guid senderId, Guid receiverId, NotificationType type, string? message, Guid? targetId)
        {
            // Don't send notification if sender and receiver are the same
            if (senderId == receiverId)
                return;

            // Create notification entity
            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Type = type,
                Message = message,
                TargetId = targetId,
                CreatedAtUtc = DateTime.UtcNow,
                IsRead = false
            };

            // Save to database
            var saved = await _notificationRepository.AddAsync(notification);

            // Get sender info for the real-time notification
            var sender = await _userRepository.GetByIdAsync(senderId, u => u.Include(x => x.Avatar));

            // Create response object matching frontend expectations
            var notificationResponse = new
            {
                Id = saved.Id,
                Type = type.Name,
                Message = message,
                TargetId = targetId,
                IsRead = false,
                CreatedAtUtc = saved.CreatedAtUtc,
                Senders = sender != null ? new[]
                {
                    new
                    {
                        Id = sender.Id,
                        FirstName = sender.FirstName,
                        LastName = sender.LastName,
                        AvatarUrl = sender.Avatar?.Key
                    }
                } : Array.Empty<object>()
            };

            // Send real-time notification via SignalR
            await _notifier.SendNotificationAsync(receiverId, notificationResponse);
        }

        public async Task<List<CommentResponse>> GetAllByRecipeAsync(Guid recipeId)
        {
            var exist = await _recipeRepository.ExistsAsync(r => r.Id == recipeId);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var comments = await _commentRepository.GetAllAsync(
                predicate: c => c.RecipeId == recipeId && c.ParentCommentId == null,
                include: c => c.Include(x => x.User)
                        .ThenInclude(x => x.Avatar)
                    .Include(x => x.Replies)
                        .ThenInclude(x => x.User)
                            .ThenInclude(x => x.Avatar)
                    .Include(x => x.Mentions)
                        .ThenInclude(x => x.MentionedUser)
                            .ThenInclude(x => x.Avatar)
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

        public async Task UpdateAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentRequest request)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bình luận không tồn tại");

            if (comment.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            comment.Content = request.Content;
            if (request.MentionedUserIds.Any())
            {
                foreach (var mentionedId in request.MentionedUserIds.Distinct())
                {
                    if (!await _userRepository.ExistsAsync(u => u.Id == mentionedId))
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION,
                            "Người dùng được nhắc tới không tồn tại.");

                    comment.Mentions.Add(new CommentMention
                    {
                        CommentId = comment.Id,
                        MentionedUserId = mentionedId
                    });
                }
            }

            await _commentRepository.UpdateAsync(comment);
            var updated = await _commentRepository.GetByIdAsync(comment.Id, c => c.Include(x => x.User).ThenInclude(x => x.Avatar));

            var response = _mapper.Map<CommentResponse>(updated);
            await _notifier.SendCommentUpdatedAsync(recipeId, response);
        }
    }
}
