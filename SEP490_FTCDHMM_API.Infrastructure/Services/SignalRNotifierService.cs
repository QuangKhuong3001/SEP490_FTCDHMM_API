using Microsoft.AspNetCore.SignalR;
using SEP490_FTCDHMM_API.Application.Interfaces.Realtime;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Hubs;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class SignalRNotifierService : IRealtimeNotifier
    {
        private readonly IHubContext<RecipeHub> _hubContext;

        public SignalRNotifierService(IHubContext<RecipeHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendCommentAsync(Guid recipeId, object comment)
        {
            await _hubContext.Clients.Group(recipeId.ToString())
                .SendAsync(HubEvent.ReceiveComment.Value, comment);
        }

        public async Task SendRatingUpdateAsync(Guid recipeId, double average)
        {
            await _hubContext.Clients.Group(recipeId.ToString())
                .SendAsync(HubEvent.ReceiveRatingUpdate.Value, average);
        }

        public async Task SendCommentDeletedAsync(Guid recipeId, Guid commentId, DateTime deletedAt)
        {
            var deleteData = new
            {
                commentId,
                recipeId,
                deletedAt
            };

            await _hubContext.Clients.Group(recipeId.ToString())
                .SendAsync(HubEvent.CommentDeleted.Value, deleteData);
        }
    }
}
