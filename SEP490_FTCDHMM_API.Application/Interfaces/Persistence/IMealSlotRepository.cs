using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IMealSlotRepository : IRepository<UserMealSlot>
    {
        Task<List<UserMealSlot>> GetByUserAsync(Guid userId);
    }
}
