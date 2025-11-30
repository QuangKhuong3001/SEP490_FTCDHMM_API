using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task CreateAsync(Guid reporterId, ReportRequest request);
        Task<ReportResponse> GetByIdAsync(Guid id);
        Task<List<ReportResponse>> GetByTargetIdAsync(Guid targetId);
        Task<PagedResult<ReportSummaryResponse>> GetSummaryAsync(ReportFilterRequest request);
        Task<(string Type, Guid TargetId)> ApproveAsync(Guid reportId, Guid adminId);
        Task RejectAsync(Guid reportId, Guid adminId, string reason);
    }

}
