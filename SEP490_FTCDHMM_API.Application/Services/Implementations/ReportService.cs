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


        public async Task<ReportResponse> GetByIdAsync(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(
                id,
                r => r.Reporter
            );

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại.");

            var dto = _mapper.Map<ReportResponse>(report);
            dto.TargetName = await ResolveTargetNameAsync(report);

            return dto;
        }

        public async Task<List<ReportResponse>> GetByTargetIdAsync(Guid targetId)
        {
            var reports = await _reportRepository.GetAllAsync(
                predicate: r => r.TargetId == targetId,
                include: q => q.Include(r => r.Reporter)
            );

            var result = new List<ReportResponse>();
            foreach (var report in reports)
            {
                var dto = _mapper.Map<ReportResponse>(report);
                dto.TargetName = await ResolveTargetNameAsync(report);
                result.Add(dto);
            }

            return result;
        }

        public async Task<PagedResult<ReportSummaryResponse>> GetSummaryAsync(ReportFilterRequest request)
        {
            //
            // 1. Get all reports with includes
            //
            var reports = await _reportRepository.GetAllAsync(
                predicate: null,
                include: q => q.Include(r => r.Reporter)
            );

            //
            // 2. Apply filters in-memory
            //
            var filtered = reports.AsEnumerable();

            if (!string.IsNullOrEmpty(request.Type))
            {
                var targetTypeValue = ReportObjectType.From(request.Type).Value;
                filtered = filtered.Where(r => r.TargetType.Value == targetTypeValue);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                var statusValue = ReportStatus.From(request.Status).Value;
                filtered = filtered.Where(r => r.Status.Value == statusValue);
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                filtered = filtered.Where(r => r.Description.Contains(request.Keyword));
            }

            //
            // 3. Convert to list
            //
            var filteredReports = filtered.ToList();

            //
            // 4. Group theo (TargetType + TargetId)
            //
            var grouped = filteredReports
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

            //
            // 5. Build danh sách DTO
            //
            var resultList = new List<ReportSummaryResponse>();

            foreach (var g in grouped)
            {
                // tạo object Report giả để reuse ResolveTargetNameAsync
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

            //
            // 6. Phân trang
            //
            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .ToList();

            //
            // 7. Return
            //
            return new PagedResult<ReportSummaryResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }



        public async Task<(string Type, Guid TargetId)> ApproveAsync(Guid reportId, Guid adminId)
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

            return (report.TargetType.Value, report.TargetId);
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




        private async Task<bool> TargetExistsAsync(ReportObjectType targetType, Guid targetId)
        {
            if (targetType == ReportObjectType.Recipe)
            {
                return await _recipeRepository.ExistsAsync(r => r.Id == targetId && !r.IsDeleted);
            }

            if (targetType == ReportObjectType.User)
            {
                return await _userRepository.ExistsAsync(u => u.Id == targetId);
            }

            if (targetType == ReportObjectType.Comment)
            {
                return await _commentRepository.ExistsAsync(c => c.Id == targetId);
            }

            if (targetType == ReportObjectType.Rating)
            {
                return await _ratingRepository.ExistsAsync(r => r.Id == targetId);
            }

            return false;
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
