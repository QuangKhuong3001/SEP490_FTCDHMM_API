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

        public async Task CreateCommentAsync(Guid userId, Guid recipeId, CreateCommentRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            Comment? parentComment = null;
            var parentCommentId = request.ParentCommentId;

            if (parentCommentId.HasValue)
            {
                parentComment = await _commentRepository.GetByIdAsync(
                    parentCommentId.Value,
                    c => c.Include(x => x.ParentComment)
                );

                if (parentComment == null)
                    throw new AppException(AppResponseCode.NOT_FOUND);

                if (parentComment.RecipeId != recipeId)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Bình luận cha không thuộc công thức này.");

                if (parentComment != null && parentComment.ParentCommentId.HasValue)
                {
                    parentComment = parentComment.ParentComment;
                }
            }

            var comment = new Comment
            {
                UserId = userId,
                RecipeId = recipeId,
                CreatedAtUtc = DateTime.UtcNow,
                Content = request.Content,
                ParentComment = parentComment
            };

            if (request.MentionedUserIds != null && request.MentionedUserIds.Any())
            {
                foreach (var mentionedId in request.MentionedUserIds.Distinct())
                {
                    if (!await _userRepository.ExistsAsync(u => u.Id == mentionedId))
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION,
                            "Người dùng được nhắc tới không tồn tại.");

                    if (mentionedId == userId)
                        throw new AppException(AppResponseCode.INVALID_ACTION,
                            "Người dùng không được nhắc tới bản thân.");

                    comment.Mentions.Add(new CommentMention
                    {
                        CommentId = comment.Id,
                        MentionedUserId = mentionedId
                    });

                    if (recipe.AuthorId != mentionedId && (parentComment == null || parentComment.UserId != mentionedId))
                    {
                        await this.CreateAndSendNotificationAsync(userId, mentionedId, NotificationType.Mention, null, recipeId);
                    }
                }
            }

            await _commentRepository.AddAsync(comment);
            var saved = await _commentRepository.GetByIdAsync(
                comment.Id,
                c => c
                    .Include(x => x.User)
                        .ThenInclude(x => x.Avatar)
                    .Include(x => x.Mentions)
                        .ThenInclude(x => x.MentionedUser)
            );

            var response = _mapper.Map<CommentResponse>(saved);

            await _notifier.SendCommentAddedAsync(recipeId, response);
            await this.CreateAndSendNotificationAsync(userId, recipe.AuthorId, NotificationType.Comment, null, recipeId);

            if (parentComment != null && parentComment.UserId != recipe.AuthorId)
            {
                await this.CreateAndSendNotificationAsync(userId, parentComment.UserId, NotificationType.Reply, null, recipeId);
            }
        }

        public async Task<List<CommentResponse>> GetCommentsByRecipeAsync(Guid recipeId)
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

        public async Task DeleteCommentAsync(Guid userId, Guid commentId, DeleteMode mode)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId,
                include: i => i.Include(c => c.Recipe)
                                .Include(c => c.Replies));
            if (comment == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

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
                var fullReply = await _commentRepository.GetByIdAsync(reply.Id,
                    include: c => c.Include(u => u.Replies));
                if (fullReply != null)
                {
                    await DeleteRepliesRecursive(fullReply);

                    await _commentRepository.DeleteAsync(fullReply);
                }
            }
        }

        public async Task UpdateCommentAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentRequest request)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId,
                include: i => i.Include(c => c.Mentions));
            if (comment == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bình luận không tồn tại");

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (comment.RecipeId != recipeId)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (comment.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            comment.Content = request.Content;
            comment.IsEdited = true;

            var oldMentionIds = comment.Mentions.Select(m => m.MentionedUserId).ToList();

            comment.Mentions.Clear();

            if (request.MentionedUserIds != null && request.MentionedUserIds.Any())
            {
                foreach (var mentionedId in request.MentionedUserIds.Distinct())
                {
                    if (!await _userRepository.ExistsAsync(u => u.Id == mentionedId))
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION,
                            "Người dùng được nhắc tới không tồn tại.");

                    if (mentionedId == userId)
                        throw new AppException(AppResponseCode.INVALID_ACTION,
                            "Người dùng không được nhắc tới bản thân.");

                    comment.Mentions.Add(new CommentMention
                    {
                        CommentId = comment.Id,
                        MentionedUserId = mentionedId
                    });

                    if (!oldMentionIds.Contains(mentionedId))
                    {
                        await this.CreateAndSendNotificationAsync(userId, mentionedId, NotificationType.Mention, null, recipeId);
                    }
                }
            }

            await _commentRepository.UpdateAsync(comment);
            var updated = await _commentRepository.GetByIdAsync(comment.Id,
                c => c.Include(x => x.User)
                .ThenInclude(x => x.Avatar));

            var response = _mapper.Map<CommentResponse>(updated);
            await _notifier.SendCommentUpdatedAsync(recipeId, response);
        }

        private async Task CreateAndSendNotificationAsync(Guid senderId, Guid receiverId, NotificationType type, string? message, Guid targetId)
        {
            if (senderId == receiverId)
                return;

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Type = type,
                Message = message,
                TargetId = targetId,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _notificationRepository.AddAsync(notification);

            var sender = await _userRepository.GetByIdAsync(senderId, u => u.Include(x => x.Avatar));

            var notificationResponse = new
            {
                Id = notification.Id,
                Type = type,
                Message = message,
                TargetId = targetId,
                IsRead = false,
                CreatedAtUtc = notification.CreatedAtUtc,
                Senders = new[]
                {
                    new
                    {
                        Id = sender!.Id,
                        FirstName = sender.FirstName,
                        LastName = sender.LastName,
                        AvatarUrl = sender.Avatar?.Key
                    }
                }
            };

            await _notifier.SendNotificationAsync(receiverId, notificationResponse);
        }
    }
}
