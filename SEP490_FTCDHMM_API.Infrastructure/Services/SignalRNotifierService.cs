using Microsoft.AspNetCore.SignalR;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Hubs;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class SignalRNotifierService : IRealtimeNotifier
    {
        private readonly IHubContext<CommentHub> _hubContext;

        public SignalRNotifierService(IHubContext<CommentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendCommentAddedAsync(Guid recipeId, object comment)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentAdded.Value, comment);
        }

        public async Task SendCommentUpdatedAsync(Guid recipeId, object comment)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentUpdated.Value, comment);
        }

        public async Task SendCommentDeletedAsync(Guid recipeId, Guid commentId)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentDeleted.Value, commentId);
        }
        public async Task SendRatingUpdateAsync(Guid recipeId, object rating)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingUpdated.Value, rating);
        }

        public async Task SendRatingDeletedAsync(Guid recipeId, Guid ratingId)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingDeleted.Value, ratingId);
        }

        public async Task SendNotificationAsync(Guid userId, object notification)
        {
            await _hubContext.Clients.Group($"user-{userId}")
                .SendAsync(HubEvent.Notification.Value, notification);
        }
        public async Task SendNotificationAsync(Guid receiverId, object notification)
        {
            await _hubContext.Clients.Group($"user-{receiverId}")
                .SendAsync(HubEvent.ReceiveNotification.Value, notification);
        }
    }
}
