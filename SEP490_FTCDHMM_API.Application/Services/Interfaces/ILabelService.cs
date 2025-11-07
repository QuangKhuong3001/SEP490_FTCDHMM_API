using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface ILabelService
    {
        Task CreatLabel(CreateLabelRequest dto);
        Task<PagedResult<LabelResponse>> GetAllLabels(LabelFilterRequest request);
        Task<IEnumerable<LabelResponse>> GetAllLabels(LabelSearchDropboxRequest request);
        Task DeleteLabel(Guid labelId);
        Task UpdateColorCode(Guid labelId, UpdateColorCodeRequest request);
    }
}
