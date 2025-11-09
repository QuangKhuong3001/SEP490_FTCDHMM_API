using Microsoft.AspNetCore.SignalR;

namespace SEP490_FTCDHMM_API.Infrastructure.Hubs
{
    public class RecipeHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var recipeId = Context.GetHttpContext()?.Request.Query["recipeId"];
            if (!string.IsNullOrEmpty(recipeId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, recipeId!);
            }

            await base.OnConnectedAsync();
        }
    }
}
