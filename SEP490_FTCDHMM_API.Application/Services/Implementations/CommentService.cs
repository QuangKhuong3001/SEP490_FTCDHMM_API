using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention;
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
        private readonly INotificationCommandService _notificationCommandService;
        private readonly ICommentMentionRepository _commentMentionRepository;

        public CommentService(
            ICommentRepository commentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            ICommentMentionRepository commentMentionRepository,
            INotificationCommandService notificationCommandService,
            IRealtimeNotifier notifier)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _commentMentionRepository = commentMentionRepository;
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _notificationCommandService = notificationCommandService;
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
                        await _notificationCommandService.CreateAndSendNotificationAsync(userId, mentionedId, NotificationType.Mention, recipeId);
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
            await _notificationCommandService.CreateAndSendNotificationAsync(userId, recipe.AuthorId, NotificationType.Comment, recipeId);

            if (parentComment != null && parentComment.UserId != recipe.AuthorId)
            {
                await _notificationCommandService.CreateAndSendNotificationAsync(userId, parentComment.UserId, NotificationType.Reply, recipeId);
            }
        }

        public async Task<List<CommentResponse>> GetCommentsByRecipeAsync(Guid recipeId)
        {
            var rootComments = await _commentRepository.GetAllAsync(
                predicate: c => c.RecipeId == recipeId && c.ParentCommentId == null,
                include: q => q
                    .AsNoTracking()
                    .Include(c => c.User)
                        .ThenInclude(u => u.Avatar)
            );

            if (!rootComments.Any())
                return new List<CommentResponse>();

            var rootIds = rootComments.Select(c => c.Id).ToList();

            var replies = await _commentRepository.GetAllAsync(
                predicate: c => c.ParentCommentId.HasValue && rootIds.Contains(c.ParentCommentId.Value),
                include: q => q
                    .AsNoTracking()
                    .Include(c => c.User)
                        .ThenInclude(u => u.Avatar)
            );

            var allCommentIds = rootComments
                .Select(c => c.Id)
                .Concat(replies.Select(r => r.Id))
                .ToList();

            var mentions = await _commentMentionRepository.GetAllAsync(
                predicate: m => allCommentIds.Contains(m.CommentId),
                include: q => q
                    .AsNoTracking()
                    .Include(m => m.MentionedUser)
                        .ThenInclude(u => u.Avatar)
            );

            var rootDtos = _mapper.Map<List<CommentResponse>>(rootComments).OrderByDescending(i => i.CreatedAtUtc).ToList();
            var replyDtos = _mapper.Map<List<CommentResponse>>(replies);
            var mentionDtos = _mapper.Map<List<MentionedUserResponse>>(mentions);

            var replyLookup = replyDtos
                .GroupBy(r => r.ParentCommentId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var root in rootDtos)
            {
                if (replyLookup.TryGetValue(root.Id, out var children))
                    root.Replies = children;
            }

            var mentionLookup = mentionDtos
                .GroupBy(m => m.CommentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            void AttachMentions(CommentResponse c)
            {
                if (mentionLookup.TryGetValue(c.Id, out var m))
                    c.Mentions = m;

                foreach (var r in c.Replies)
                    AttachMentions(r);
            }

            foreach (var root in rootDtos)
                AttachMentions(root);

            return rootDtos;
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
                        await _notificationCommandService.CreateAndSendNotificationAsync(userId, mentionedId, NotificationType.Mention, recipeId);
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
    }
}
