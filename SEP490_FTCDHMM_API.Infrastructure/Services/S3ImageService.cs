using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Domain.Constants;
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

        public async Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder)
        {
            try
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
                    CreatedAtUTC = DateTime.UtcNow
                };

                _dbContext.Images.Add(image);
                await _dbContext.SaveChangesAsync();

                return image;
            }
            catch (Exception e)
            {
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE, $"S3 storage đang không hoạt động - {e}");
            }
        }

        public async Task<Image> UploadImageAsync(FileUploadModel file, StorageFolder folder)
        {
            try
            {
                if (!IsImage(file))
                    throw new AppException(AppResponseCode.INVALID_FILE);

                var id = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName);
                var key = $"{folder}/{id}{extension}";

                var transfer = new TransferUtility(_s3Client);
                await transfer.UploadAsync(file.Content, _settings.BucketName, key);

                var image = new Image
                {
                    Id = id,
                    Key = key,
                    ContentType = file.ContentType,
                    CreatedAtUTC = DateTime.UtcNow
                };

                await _dbContext.Images.AddAsync(image);
                await _dbContext.SaveChangesAsync();

                return image;
            }
            catch (Exception e)
            {
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE, $"S3 storage đang không hoạt động - {e}");
            }
        }

        public async Task<List<Image>> UploadImagesAsync(IEnumerable<IFormFile> files, StorageFolder folder)
        {
            try
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
                        CreatedAtUTC = DateTime.UtcNow
                    };

                    images.Add(image);
                    _dbContext.Images.Add(image);
                }

                await _dbContext.SaveChangesAsync();

                return images;
            }
            catch (Exception e)
            {
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE, $"S3 storage đang không hoạt động - {e}");
            }
        }

        public async Task<List<Image>> UploadImagesAsync(IReadOnlyList<FileUploadModel> files, StorageFolder folder)
        {
            try
            {
                var result = new List<Image>();

                foreach (var f in files)
                {
                    var img = await UploadImageAsync(f, folder);
                    result.Add(img);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE, $"S3 storage đang không hoạt động - {e}");
            }
        }


        public string? GeneratePreSignedUrl(string? key)
        {
            var expires = DateTime.UtcNow.AddDays(_settings.ExpiryDays);

            var defaultImageRequest = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = DefaultValues.DEFAULT_IMAGE_KEY,
                Expires = expires
            };

            if (string.IsNullOrWhiteSpace(key))
                return _s3Client.GetPreSignedURL(defaultImageRequest);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddDays(_settings.ExpiryDays)
            };

            var result = _s3Client.GetPreSignedURL(request);

            if (result == null)
            {
                result = _s3Client.GetPreSignedURL(defaultImageRequest);
            }

            return result;
        }

        public async Task<Image> MirrorExternalImageAsync(StorageFolder folder, string url)
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
                CreatedAtUTC = DateTime.UtcNow,
            };

            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync();
            return image;
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            try
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

                image.IsDeleted = true;
                _dbContext.Images.Update(image);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE, $"S3 storage đang không hoạt động - {e}");
            }
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

        private bool IsImage(FileUploadModel file)
        {
            if (string.IsNullOrWhiteSpace(file.ContentType))
                return false;

            return file.ContentType.StartsWith("image/");
        }
    }
}
