using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly AppDbContext _dbContext;
        public S3Service(IAmazonS3 s3Client, string bucketName, AppDbContext dbContext)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
            _dbContext = dbContext;
        }

        public async Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder, AppUser uploadedBy)
        {
            var extension = Path.GetExtension(file.FileName);
            var key = $"{folder}/{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(stream, _bucketName, key);

            var image = new Image
            {
                Id = Guid.NewGuid(),
                Key = key,
                FileName = file.FileName,
                ContentType = file.ContentType,
                UploadedBy = uploadedBy,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync();

            return image;
        }

        public async Task<List<Image>> UploadImagesAsync(IEnumerable<IFormFile> files, StorageFolder folder, AppUser uploadedBy)
        {
            var images = new List<Image>();
            var transfer = new TransferUtility(_s3Client);

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName);
                var key = $"{folder}/{Guid.NewGuid()}{extension}";

                using var stream = file.OpenReadStream();
                await transfer.UploadAsync(stream, _bucketName, key);

                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    UploadedBy = uploadedBy,
                    CreatedAt = DateTime.UtcNow
                };

                images.Add(image);
                _dbContext.Images.Add(image);
            }

            await _dbContext.SaveChangesAsync();

            return images;
        }
    }
}
