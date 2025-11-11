using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
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

        public async Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder, AppUser? uploadedBy)
        {
            if (!IsImage(file))
                throw new AppException(AppResponseCode.INVALID_FILE);

            var id = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var key = $"{folder}/{id}{extension}";

            using var stream = file.OpenReadStream();
            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(stream, _settings.BucketName, key);

            var image = new Image
            {
                Id = id,
                Key = key,
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
            var id = Guid.NewGuid();

            foreach (var file in files)
            {
                if (!IsImage(file))
                    throw new AppException(AppResponseCode.INVALID_FILE);

                var extension = Path.GetExtension(file.FileName);
                var key = $"{folder}/{id}{extension}";

                using var stream = file.OpenReadStream();
                await transfer.UploadAsync(stream, _settings.BucketName, key);

                var image = new Image
                {
                    Id = id,
                    Key = key,
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

        public async Task<Image> MirrorExternalImageAsync(StorageFolder folder, string url, Guid uploadedById)
        {
            using var http = new HttpClient();
            using var resp = await http.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var contentType = resp.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var bytes = await resp.Content.ReadAsByteArrayAsync();

            var id = Guid.NewGuid();
            var ext = contentType switch
            {
                "image/png" => ".png",
                "image/gif" => ".gif",
                _ => ".jpg"
            };
            var fileName = $"{id}{ext}";
            var key = $"{folder}/{fileName}";

            using var ms = new MemoryStream(bytes);
            var upload = new TransferUtilityUploadRequest
            {
                InputStream = ms,
                Key = key,
                BucketName = _settings.BucketName,
                ContentType = contentType,
            };
            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(upload);

            var image = new Image
            {
                Id = id,
                Key = key,
                ContentType = contentType,
                CreatedAt = DateTime.UtcNow,
                UploadedById = uploadedById
            };

            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync();
            return image;
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null)
                throw new Exception("Image not found");

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = image.Key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);

            _dbContext.Images.Remove(image);
            await _dbContext.SaveChangesAsync();
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

            // GIF
            if (header[0] == 0x47 && header[1] == 0x49 &&
                header[2] == 0x46 && header[3] == 0x38) return true;

            return false;
        }

    }
}
