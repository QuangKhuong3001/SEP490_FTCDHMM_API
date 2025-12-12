using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IRatingRepository _ratingRepository;

        public ReportService(
            IReportRepository reportRepository,
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            ICommentRepository commentRepository,
            IRatingRepository ratingRepository)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
        }



        public async Task CreateReportAsync(Guid reporterId, ReportRequest request)
        {
            var reporter = await _userRepository.GetByIdAsync(reporterId);

            var targetType = ReportObjectType.From(request.TargetType);

            var existingReport = await _reportRepository.FirstOrDefaultAsync(
                orderByDescendingKeySelector: r => r.CreatedAtUtc,
                predicate: r =>
                r.ReporterId == reporterId &&
                r.TargetId == request.TargetId &&
                r.TargetType == targetType &&
                r.Status == ReportStatus.Pending);


            if (existingReport != null)
            {
                if (request.Description == null || request.Description.Trim().IsNullOrEmpty())
                    return;

                if (request.Description == existingReport.Description)
                    return;

                existingReport.Description = request.Description;
                await _reportRepository.UpdateAsync(existingReport);
                return;
            }

            if (reporterId == request.TargetId && (request.TargetType.ToUpperInvariant() == ReportObjectType.User.Value))
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể báo cáo chính mình.");

            await this.TargetExistsAsync(ReportObjectType.From(request.TargetType), request.TargetId);

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


        public async Task<ReportDetailListResponse> GetReportDetailsAsync(Guid targetId, string targetType)
        {
            var reports = await _reportRepository.GetAllAsync(
                predicate: r => r.TargetId == targetId && r.TargetType == ReportObjectType.From(targetType),
                include: q => q.Include(r => r.Reporter)
                    );

            if (!reports.Any())
                throw new AppException(AppResponseCode.NOT_FOUND, "Không có báo cáo nào cho đối tượng này.");

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


        public async Task<PagedResult<ReportsResponse>> GetReportsAsync(ReportFilterRequest request)
        {
            var type = ReportObjectType.Recipe;
            if (request.Type != null)
            {
                type = ReportObjectType.From(request.Type);
            }

            Expression<Func<Report, bool>> filter = r =>
                r.Status == ReportStatus.Pending &&
                (string.IsNullOrEmpty(request.Type) ||
                r.TargetType == type);


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

            var resultList = new List<ReportsResponse>();
            var keywordNormalized = request.Keyword?.NormalizeVi() ?? "";

            foreach (var g in grouped)
            {
                var temp = new Report
                {
                    TargetId = g.TargetId,
                    TargetType = ReportObjectType.From(g.TargetType)
                };

                var targetName = await ResolveTargetNameAsync(temp);
                var normalizedTargetName = targetName.NormalizeVi();

                if (normalizedTargetName.Contains(keywordNormalized))
                {
                    resultList.Add(new ReportsResponse
                    {
                        TargetType = g.TargetType,
                        TargetId = g.TargetId,
                        TargetName = targetName,
                        Count = g.Count,
                        LatestReportAtUtc = g.LatestAt
                    });
                }
            }

            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .ToList();

            return new PagedResult<ReportsResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }
        public async Task<PagedResult<ReportsResponse>> GetReportHistoriesAsync(ReportFilterRequest request)
        {
            var type = ReportObjectType.Recipe;

            if (!string.IsNullOrEmpty(request.Type))
                type = ReportObjectType.From(request.Type);

            Expression<Func<Report, bool>> filter = r =>
                (r.Status == ReportStatus.Approved || r.Status == ReportStatus.Rejected) &&
                (string.IsNullOrEmpty(request.Type) ||
                    r.TargetType == type);

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

            var resultList = new List<ReportsResponse>();
            var keywordNormalized = request.Keyword?.NormalizeVi() ?? "";

            foreach (var g in grouped)
            {
                var temp = new Report
                {
                    TargetId = g.TargetId,
                    TargetType = ReportObjectType.From(g.TargetType)
                };

                var targetName = await ResolveTargetNameAsync(temp);
                var normalizedTargetName = targetName.NormalizeVi();

                if (normalizedTargetName.Contains(keywordNormalized))
                {
                    resultList.Add(new ReportsResponse
                    {
                        TargetType = g.TargetType,
                        TargetId = g.TargetId,
                        TargetName = targetName,
                        Count = g.Count,
                        LatestReportAtUtc = g.LatestAt
                    });
                }
            }

            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .ToList();

            return new PagedResult<ReportsResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task ApproveReportAsync(Guid reportId, Guid userId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Báo cáo không tồn tại.");

            if (report.Status != ReportStatus.Pending)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Báo cáo đã được xử lí trước đó.");

            report.Status = ReportStatus.Approved;
            report.ReviewedBy = userId;
            report.ReviewedAtUtc = DateTime.UtcNow;

            await _reportRepository.UpdateAsync(report);
        }


        public async Task RejectReportAsync(Guid reportId, Guid userId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new AppException(AppResponseCode.INVALID_ACTION, "Lý do từ chối không được để trống.");

            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Báo cáo không tồn tại.");

            if (report.Status != ReportStatus.Pending)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Báo cáo đã được xử lí trước đó.");

            report.Status = ReportStatus.Rejected;
            report.ReviewedBy = userId;
            report.ReviewedAtUtc = DateTime.UtcNow;
            report.RejectReason = reason;

            await _reportRepository.UpdateAsync(report);
        }

        private async Task TargetExistsAsync(ReportObjectType targetType, Guid targetId)
        {
            var exists = false;
            if (targetType == ReportObjectType.Recipe)
            {
                exists = await _recipeRepository.ExistsAsync(r => r.Id == targetId && r.Status == RecipeStatus.Posted);
            }
            else if (targetType == ReportObjectType.User)
            {
                exists = await _userRepository.ExistsAsync(u => u.Id == targetId);
            }
            else if (targetType == ReportObjectType.Comment)
            {
                exists = await _commentRepository.ExistsAsync(c => c.Id == targetId);
            }
            else if (targetType == ReportObjectType.Rating)
            {
                exists = await _ratingRepository.ExistsAsync(r => r.Id == targetId);
            }
            else
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Loại đối tượng báo cáo không hợp lệ.");
            }

            if (!exists)
            {
                throw new AppException(AppResponseCode.NOT_FOUND, "Đối tượng báo cáo không tồn tại.");
            }
        }

        private async Task<string> ResolveTargetNameAsync(Report report)
        {
            if (report.TargetType == ReportObjectType.Recipe)
            {
                var recipe = await _recipeRepository.GetByIdAsync(report.TargetId);
                if (recipe == null)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại.");

                return recipe.Name;
            }

            if (report.TargetType == ReportObjectType.User)
            {
                var user = await _userRepository.GetByIdAsync(report.TargetId);
                if (user == null)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Người dùng không tồn tại.");

                var first = user.FirstName?.Trim() ?? "";
                var last = user.LastName?.Trim() ?? "";
                var fullName = $"{first} {last}".Trim();

                return fullName;
            }

            if (report.TargetType == ReportObjectType.Comment)
            {
                var comment = await _commentRepository.GetByIdAsync(report.TargetId);
                if (comment == null)
                    return "[Bình luận đã bị xóa]";

                return comment.Content;
            }

            if (report.TargetType == ReportObjectType.Rating)
            {
                var rating = await _ratingRepository.GetByIdAsync(report.TargetId);
                if (rating == null)
                    return "[Đánh giá đã bị xóa]";

                var feedback = rating.Feedback?.Trim();

                if (string.IsNullOrWhiteSpace(feedback))
                    return $"{rating.Score} stars";

                return $"{rating.Score} stars — {feedback}";
            }
            return string.Empty;
        }
    }
}
