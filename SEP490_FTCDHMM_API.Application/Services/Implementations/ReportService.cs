using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            ICommentRepository commentRepository,
            IRatingRepository ratingRepository,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }



        public async Task CreateAsync(Guid reporterId, ReportRequest request)
        {
            var reporter = await _userRepository.GetByIdAsync(reporterId);
            if (reporter == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, "Tài khoản không tồn tại.");

            if (reporterId == request.TargetId && request.TargetType.Trim().ToUpperInvariant() == "USER")
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể report chính mình.");

            await this.TargetExistsAsync(ReportObjectType.From(request.TargetType), request.TargetId);

            var targetType = ReportObjectType.From(request.TargetType);

            var description = request.Description?.Trim() ?? string.Empty;

            var report = new Report
            {
                Id = Guid.NewGuid(),
                ReporterId = reporterId,
                TargetId = request.TargetId,
                TargetType = targetType,
                Description = description,
                Status = ReportStatus.Pending,
                ReviewedBy = null,
                ReviewedAtUtc = null,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _reportRepository.AddAsync(report);
        }


        public async Task<ReportDetailListResponse> GetDetailAsync(Guid targetId, string targetType)
        {
            var reports = await _reportRepository.GetAllAsync(
                predicate: r => r.TargetId == targetId && r.TargetType == ReportObjectType.From(targetType),
                include: q => q.Include(r => r.Reporter)
                    );



            if (!reports.Any())
                throw new AppException(AppResponseCode.NOT_FOUND, "Không có report nào cho đối tượng này.");

            var sample = reports.First();
            var targetName = await ResolveTargetNameAsync(sample);

            var detailItems = reports
                .OrderByDescending(r => r.CreatedAtUtc)
                .Select(r => new ReportDetailItem
                {
                    ReportId = r.Id,
                    ReporterName = $"{r.Reporter.FirstName} {r.Reporter.LastName}".Trim(),
                    Description = r.Description,
                    Status = r.Status.Value,
                    CreatedAtUtc = r.CreatedAtUtc
                })
                .ToList();

            return new ReportDetailListResponse
            {
                TargetId = targetId,
                TargetType = targetType,
                TargetName = targetName,
                Reports = detailItems
            };
        }


        public async Task<PagedResult<ReportSummaryResponse>> GetSummaryAsync(ReportFilterRequest request)
        {
            Expression<Func<Report, bool>> filter = r =>
                r.Status == ReportStatus.Pending &&
                (string.IsNullOrEmpty(request.Type) ||
                r.TargetType.Value == ReportObjectType.From(request.Type).Value)
                    &&
                        (string.IsNullOrEmpty(request.Keyword) ||
                        r.Description.Contains(request.Keyword));


            Func<IQueryable<Report>, IQueryable<Report>> include = q => q.Include(r => r.Reporter);

            var reports = await _reportRepository.GetAllAsync(
                predicate: filter,
                include: include
            );

            var grouped = reports
                .GroupBy(r => new { r.TargetType.Value, r.TargetId })
                .Select(g => new
                {
                    TargetType = g.Key.Value,
                    TargetId = g.Key.TargetId,
                    Count = g.Count(),
                    LatestAt = g.Max(x => x.CreatedAtUtc)
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.LatestAt)
                .ToList();

            var resultList = new List<ReportSummaryResponse>();

            foreach (var g in grouped)
            {
                var temp = new Report
                {
                    TargetId = g.TargetId,
                    TargetType = ReportObjectType.From(g.TargetType)
                };

                var targetName = await ResolveTargetNameAsync(temp);

                resultList.Add(new ReportSummaryResponse
                {
                    TargetType = g.TargetType,
                    TargetId = g.TargetId,
                    TargetName = targetName,
                    Count = g.Count,
                    LatestReportAtUtc = g.LatestAt
                });
            }

            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .ToList();

            return new PagedResult<ReportSummaryResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }
        public async Task<PagedResult<ReportSummaryResponse>> GetHistoryAsync(ReportFilterRequest request)
        {
            Expression<Func<Report, bool>> filter = r =>
                (r.Status == ReportStatus.Approved || r.Status == ReportStatus.Rejected) &&
                (string.IsNullOrEmpty(request.Type) ||
                    r.TargetType.Value == ReportObjectType.From(request.Type).Value) &&
                (string.IsNullOrEmpty(request.Keyword) ||
                    r.Description.Contains(request.Keyword));

            Func<IQueryable<Report>, IQueryable<Report>> include = q => q.Include(r => r.Reporter);

            var reports = await _reportRepository.GetAllAsync(
                predicate: filter,
                include: include
            );

            var grouped = reports
                .GroupBy(r => new { r.TargetType.Value, r.TargetId })
                .Select(g => new
                {
                    TargetType = g.Key.Value,
                    TargetId = g.Key.TargetId,
                    Count = g.Count(),
                    LatestAt = g.Max(x => x.ReviewedAtUtc) ?? g.Max(x => x.CreatedAtUtc)
                })
                .OrderByDescending(x => x.LatestAt)
                .ToList();

            var resultList = new List<ReportSummaryResponse>();

            foreach (var g in grouped)
            {
                var temp = new Report
                {
                    TargetId = g.TargetId,
                    TargetType = ReportObjectType.From(g.TargetType)
                };

                var targetName = await ResolveTargetNameAsync(temp);

                resultList.Add(new ReportSummaryResponse
                {
                    TargetType = g.TargetType,
                    TargetId = g.TargetId,
                    TargetName = targetName,
                    Count = g.Count,
                    LatestReportAtUtc = g.LatestAt
                });
            }

            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .ToList();

            return new PagedResult<ReportSummaryResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task ApproveAsync(Guid reportId, Guid adminId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại.");

            if (report.Status == ReportStatus.Approved)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Report đã được duyệt trước đó.");

            report.Status = ReportStatus.Approved;
            report.ReviewedBy = adminId;
            report.ReviewedAtUtc = DateTime.UtcNow;

            await _reportRepository.UpdateAsync(report);
        }


        public async Task RejectAsync(Guid reportId, Guid adminId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new AppException(AppResponseCode.INVALID_ACTION, "Lý do từ chối không được để trống.");

            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại.");

            if (report.Status == ReportStatus.Rejected)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Report đã bị từ chối trước đó.");

            report.Status = ReportStatus.Rejected;
            report.ReviewedBy = adminId;
            report.ReviewedAtUtc = DateTime.UtcNow;
            report.RejectReason = reason;

            await _reportRepository.UpdateAsync(report);
        }

        private async Task TargetExistsAsync(ReportObjectType targetType, Guid targetId)
        {
            bool exists = targetType switch
            {
                var t when t == ReportObjectType.Recipe =>
                    await _recipeRepository.ExistsAsync(r => r.Id == targetId && r.Status == RecipeStatus.Posted),

                var t when t == ReportObjectType.User =>
                    await _userRepository.ExistsAsync(u => u.Id == targetId),

                var t when t == ReportObjectType.Comment =>
                    await _commentRepository.ExistsAsync(c => c.Id == targetId),

                var t when t == ReportObjectType.Rating =>
                    await _ratingRepository.ExistsAsync(r => r.Id == targetId),

                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Loại đối tượng report không hợp lệ.")
            };

            if (!exists)
                throw new AppException(AppResponseCode.NOT_FOUND, "Đối tượng bị report không tồn tại.");
        }


        private async Task<string> ResolveTargetNameAsync(Report report)
        {
            if (report.TargetType == ReportObjectType.Recipe)
            {
                var recipe = await _recipeRepository.GetByIdAsync(report.TargetId);
                return recipe?.Name?.Trim() ?? "Unknown Recipe";
            }

            if (report.TargetType == ReportObjectType.User)
            {
                var user = await _userRepository.GetByIdAsync(report.TargetId);
                if (user == null)
                    return "Unknown User";

                var first = user.FirstName?.Trim() ?? "";
                var last = user.LastName?.Trim() ?? "";
                var full = $"{first} {last}".Trim();

                return string.IsNullOrWhiteSpace(full) ? "Unknown User" : full;
            }

            if (report.TargetType == ReportObjectType.Comment)
            {
                var comment = await _commentRepository.GetByIdAsync(report.TargetId);
                var content = comment?.Content?.Trim();

                if (string.IsNullOrWhiteSpace(content))
                    return "(no content)";

                return content;
            }

            if (report.TargetType == ReportObjectType.Rating)
            {
                var rating = await _ratingRepository.GetByIdAsync(report.TargetId);
                if (rating == null)
                    return "Unknown Rating";

                var feedback = rating.Feedback?.Trim();

                if (string.IsNullOrWhiteSpace(feedback))
                    return $"{rating.Score} stars";

                return $"{rating.Score} stars — {feedback}";
            }

            return "Unknown";
        }


    }
}
