namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IUserMealSlotInitializer
    {
        Task InitializeDefaultAsync(Guid userId);
    }
}
