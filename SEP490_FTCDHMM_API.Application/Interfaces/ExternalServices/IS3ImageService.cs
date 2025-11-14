using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IS3ImageService
    {
        Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder, AppUser? uploadedBy);
        Task<Image> UploadImageAsync(FileUploadModel file, StorageFolder folder, Guid? userId);
        Task<List<Image>> UploadImagesAsync(IEnumerable<IFormFile> files, StorageFolder folder, AppUser uploadedBy);
        Task<List<Image>> UploadImagesAsync(IReadOnlyList<FileUploadModel> files, StorageFolder folder, Guid? userId);
        string? GeneratePreSignedUrl(string? key);
        Task<Image> MirrorExternalImageAsync(StorageFolder folder, string url, Guid uploadedById);
        Task DeleteImageAsync(Guid imageId);
    }
}
