using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class ImageRepository : EfRepository<Image>, IImageRepository
    {
        private readonly AppDbContext _dbContext;

        public ImageRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MarkDeletedAsync(Guid? imageId)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null)
                return;

            image.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkDeletedAsync(List<Guid> imageIds)
        {
            var ids = imageIds.Distinct().ToList();

            var images = await _dbContext.Images
                .Where(i => ids.Contains(i.Id))
                .ToListAsync();

            if (images.Count == 0)
                return;

            foreach (var img in images)
            {
                img.IsDeleted = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkDeletedStepsImageFromDraftAsync(DraftRecipe draft)
        {
            var imageIds = new List<Guid>();

            if (draft.ImageId != null)
                imageIds.Add(draft.ImageId.Value);

            imageIds.AddRange(
                draft.DraftCookingSteps
                    .SelectMany(s => s.DraftCookingStepImages)
                    .Select(i => i.ImageId)
            );

            imageIds = imageIds.Distinct().ToList();

            await MarkDeletedAsync(imageIds);
        }

    }
}
