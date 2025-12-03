using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Jobs.Implementations
{
    public class DeletedImagesJob : IDeletedImagesJob
    {
        private readonly IImageRepository _imageRepository;

        public DeletedImagesJob(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;

            var deleted = await _imageRepository.GetAllAsync(
                img => img.IsDeleted
            );

            if (!deleted.Any())
            {
                return;
            }

            await _imageRepository.DeleteRangeAsync(deleted);
        }
    }
}
