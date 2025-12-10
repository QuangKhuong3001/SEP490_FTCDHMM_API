namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IRealtimeNotifier
    {
        Task SendCommentAddedAsync(Guid recipeId, object comment);
        Task SendCommentUpdatedAsync(Guid recipeId, object comment);
        Task SendCommentDeletedAsync(Guid recipeId, Guid commentId);
        Task SendRatingUpdateAsync(Guid recipeId, object rating);
        Task SendRatingDeletedAsync(Guid recipeId, Guid ratingId);
        Task SendNotificationAsync(Guid userId, object notification);
        Task SendMarkNotificationAsReadAsync(Guid userId, Guid notificationId);
    }
}
