using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<string> GetAvatarKeyByUserId(Guid userId);
    }
}
