using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task CreateReportAsync(Guid reporterId, ReportRequest request);
        Task<ReportDetailListResponse> GetReportDetailsAsync(Guid targetId, string targetType);
        Task<PagedResult<ReportsResponse>> GetReportsAsync(ReportFilterRequest request);
        Task<PagedResult<ReportsResponse>> GetReportHistoriesAsync(ReportFilterRequest request);
        Task ApproveReportAsync(Guid reportId, Guid userId);
        Task RejectReportAsync(Guid reportId, Guid userId, string reason);
    }

}

