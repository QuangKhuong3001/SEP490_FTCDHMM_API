using Microsoft.AspNetCore.SignalR;

namespace SEP490_FTCDHMM_API.Infrastructure.Hubs
{
    public class CommentHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var recipeId = Context.GetHttpContext()?.Request.Query["recipeId"];
            if (!string.IsNullOrEmpty(recipeId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"recipe-{recipeId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var recipeId = Context.GetHttpContext()?.Request.Query["recipeId"];
            if (!string.IsNullOrEmpty(recipeId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"recipe-{recipeId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
