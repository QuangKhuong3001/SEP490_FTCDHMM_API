using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<bool> CreateReportAsync(Guid reporterId, ReportRequest request);
        Task<IReadOnlyList<ReportResponse>> GetAllPendingAsync();
        Task<IReadOnlyList<ReportResponse>> GetReportsByTypeAsync(string type);
        Task<ReportResponse> GetByIdAsync(Guid id);
        Task<bool> ApproveAsync(Guid reportId, Guid adminId);
        Task<bool> RejectAsync(Guid reportId, Guid adminId);
        Task<bool> DeleteAsync(Guid reportId);
    }
}

