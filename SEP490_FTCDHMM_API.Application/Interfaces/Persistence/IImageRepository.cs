using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IImageRepository : IRepository<Image>
    {
        Task MarkDeletedAsync(Guid? imageId);
        Task MarkDeletedAsync(List<Guid> imageIds);
        Task MarkDeletedStepsImageFromDraftAsync(DraftRecipe draft);
    }
}
