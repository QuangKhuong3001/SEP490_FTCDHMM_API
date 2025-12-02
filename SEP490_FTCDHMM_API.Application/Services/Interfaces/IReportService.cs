using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task CreateAsync(Guid reporterId, ReportRequest request);
        Task<ReportResponse> GetByIdAsync(Guid id);
        Task<PagedResult<ReportSummaryResponse>> GetSummaryAsync(ReportFilterRequest request);
        Task ApproveAsync(Guid reportId, Guid adminId);
        Task RejectAsync(Guid reportId, Guid adminId, string reason);
    }

}

