using Microsoft.AspNetCore.SignalR;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
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
        public async Task SendCommentDeletedAsync(Guid recipeId, Guid commentId)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.CommentDeleted.Value, commentId);
        }
        public async Task SendRatingUpdateAsync(Guid recipeId, object raiting)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingUpdated.Value, raiting);
        }

        public async Task SendRatingDeletedAsync(Guid recipeId, Guid raitingId)
        {
            await _hubContext.Clients.Group($"recipe-{recipeId}")
                .SendAsync(HubEvent.RatingDeleted.Value, raitingId);
        }
    }
}
