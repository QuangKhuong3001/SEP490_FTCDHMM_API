using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Data;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class S3ImageService : IS3ImageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsS3Settings _settings;
        private readonly AppDbContext _dbContext;

        public S3ImageService(
            IAmazonS3 s3Client,
            IOptions<AwsS3Settings> options,
            AppDbContext dbContext)
        {
            _s3Client = s3Client;
            _settings = options.Value;
            _dbContext = dbContext;
        }

        public async Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder, AppUser uploadedBy)
        {
            if (!IsImage(file))
                throw new AppException(AppResponseCode.INVALID_FILE);

            var extension = Path.GetExtension(file.FileName);
            var key = $"{folder}/{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(stream, _settings.BucketName, key);

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
                if (!IsImage(file))
                    throw new AppException(AppResponseCode.INVALID_FILE);

                var extension = Path.GetExtension(file.FileName);
                var key = $"{folder}/{Guid.NewGuid()}{extension}";

                using var stream = file.OpenReadStream();
                await transfer.UploadAsync(stream, _settings.BucketName, key);

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

        public string? GeneratePreSignedUrl(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddDays(_settings.ExpiryDays)
            };

            return _s3Client.GetPreSignedURL(request);
        }

        private bool IsImage(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            byte[] header = new byte[8];
            stream.Read(header, 0, header.Length);

            // JPG
            if (header[0] == 0xFF && header[1] == 0xD8) return true;

            // PNG
            if (header[0] == 0x89 && header[1] == 0x50 &&
                header[2] == 0x4E && header[3] == 0x47) return true;

            return false;
        }

    }
}
