using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IS3Service
    {
        Task<Image> UploadImageAsync(IFormFile file, StorageFolder folder, AppUser uploadedBy);
        Task<List<Image>> UploadImagesAsync(IEnumerable<IFormFile> files, StorageFolder folder, AppUser uploadedBy);
    }
}
