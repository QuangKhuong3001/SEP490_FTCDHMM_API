using Microsoft.AspNetCore.SignalR;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Hubs;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class SignalRNotifierService : IRealtimeNotifier
    {
        private readonly IHubContext<CommentHub> _commentHubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public SignalRNotifierService(IHubContext<CommentHub> commentHubContext, IHubContext<NotificationHub> notificationHubContext)
        {
            _commentHubContext = commentHubContext;
            _notificationHubContext = notificationHubContext;
        }

        public async Task SendCommentAddedAsync(Guid recipeId, object comment)
        {
            await _commentHubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentAdded.Value, comment);
        }

        public async Task SendCommentUpdatedAsync(Guid recipeId, object comment)
        {
            await _commentHubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentUpdated.Value, comment);
        }

        public async Task SendCommentDeletedAsync(Guid recipeId, Guid commentId)
        {
            await _commentHubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentDeleted.Value, commentId);
        }
        public async Task SendRatingUpdateAsync(Guid recipeId, object rating)
        {
            await _commentHubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingUpdated.Value, rating);
        }

        public async Task SendRatingDeletedAsync(Guid recipeId, Guid ratingId)
        {
            await _commentHubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingDeleted.Value, ratingId);
        }

        public async Task SendNotificationAsync(Guid userId, object notification)
        {
            await _notificationHubContext.Clients.Group($"user-{userId}")
                 .SendAsync("NOTIFICATION", notification);
        }
    }
}
