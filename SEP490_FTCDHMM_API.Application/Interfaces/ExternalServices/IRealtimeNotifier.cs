namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IRealtimeNotifier
    {
        Task SendCommentAsync(Guid recipeId, object comment);
        Task SendRatingUpdateAsync(Guid recipeId, double average);
        Task SendCommentDeletedAsync(Guid recipeId, Guid commentId, DateTime deletedAt);
        Task SendNotificationAsync(Guid receiverId, object notification);
    }
}
