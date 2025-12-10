using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface ILabelService
    {
        Task CreatLabelAsync(CreateLabelRequest dto);
        Task<PagedResult<LabelResponse>> GetPagedLabelsAsync(LabelFilterRequest request);
        Task<IEnumerable<LabelResponse>> GetLabelsAsync(LabelSearchDropboxRequest request);
        Task DeleteLabelAsync(Guid labelId);
        Task UpdateColorCodeAsync(Guid labelId, UpdateColorCodeRequest request);
    }
}
