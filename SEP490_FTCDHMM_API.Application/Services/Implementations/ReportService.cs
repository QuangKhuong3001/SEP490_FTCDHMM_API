using AutoMapper;
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
        private readonly IReportRepository _reportRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepo,
            IUserRepository userRepo,
            IRecipeRepository recipeRepo,
            ICommentRepository commentRepo,
            IRatingRepository ratingRepo,
            IMapper mapper)
        {
            _reportRepo = reportRepo;
            _userRepo = userRepo;
            _recipeRepo = recipeRepo;
            _commentRepo = commentRepo;
            _ratingRepo = ratingRepo;
            _mapper = mapper;
        }

        // ========================================================
        // CREATE
        // ========================================================
        public async Task<bool> CreateReportAsync(Guid reporterId, ReportRequest request)
        {
            if (reporterId == request.TargetId)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể report chính mình");

            var report = new Report
            {
                ReporterId = reporterId,
                TargetId = request.TargetId,
                TargetType = ReportObjectType.From(request.TargetType),
                Description = request.Description ?? "",
                Status = ReportStatus.Pending,
                ReviewedBy = null,
                ReviewedAtUtc = null,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _reportRepo.AddAsync(report);
            return true;
        }

        // ========================================================
        // GET BY ID
        // ========================================================
        public async Task<ReportResponse> GetByIdAsync(Guid id)
        {
            var report = await _reportRepo.GetByIdAsync(id);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại");

            var dto = _mapper.Map<ReportResponse>(report);
            dto.TargetName = await ResolveTargetName(report);

            return dto;
        }

        // ========================================================
        // GET BY TYPE (sửa để dùng ==)
        // ========================================================
        public async Task<IReadOnlyList<ReportResponse>> GetReportsByTypeAsync(string type)
        {
            var targetType = ReportObjectType.From(type);

            var reports = await _reportRepo.GetAllAsync(r => r.TargetType == targetType);

            return await ConvertToListAsync(reports);
        }

        // ========================================================
        // GET ALL PENDING (sửa sang ==)
        // ========================================================
        public async Task<IReadOnlyList<ReportResponse>> GetAllPendingAsync()
        {
            var reports = await _reportRepo.GetAllAsync(r => r.Status == ReportStatus.Pending);
            return await ConvertToListAsync(reports);
        }

        // ========================================================
        // APPROVE
        // ========================================================
        public async Task<bool> ApproveAsync(Guid reportId, Guid adminId)
        {
            var report = await _reportRepo.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại");

            if (report.Status == ReportStatus.Approved)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Report đã được duyệt");

            report.Status = ReportStatus.Approved;
            report.ReviewedBy = adminId;
            report.ReviewedAtUtc = DateTime.UtcNow;

            await _reportRepo.UpdateAsync(report);

            await HandleApproveAction(report);

            return true;
        }

        // ========================================================
        // REJECT
        // ========================================================
        public async Task<bool> RejectAsync(Guid reportId, Guid adminId)
        {
            var report = await _reportRepo.GetByIdAsync(reportId);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại");

            if (report.Status == ReportStatus.Rejected)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Report đã bị từ chối trước đó");

            report.Status = ReportStatus.Rejected;
            report.ReviewedBy = adminId;
            report.ReviewedAtUtc = DateTime.UtcNow;

            await _reportRepo.UpdateAsync(report);
            return true;
        }

        // ========================================================
        // DELETE
        // ========================================================
        public async Task<bool> DeleteAsync(Guid id)
        {
            var report = await _reportRepo.GetByIdAsync(id);

            if (report == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Report không tồn tại");

            await _reportRepo.DeleteAsync(report);
            return true;
        }

        // ========================================================
        // PRIVATE HELPERS (refactor sang ==)
        // ========================================================
        private async Task<string> ResolveTargetName(Report report)
        {
            // --- RECIPE ---
            if (report.TargetType == ReportObjectType.Recipe)
            {
                var recipe = await _recipeRepo.GetByIdAsync(report.TargetId);
                return recipe?.Name?.Trim() ?? "Unknown Recipe";
            }

            // --- USER ---
            if (report.TargetType == ReportObjectType.User)
            {
                var user = await _userRepo.GetByIdAsync(report.TargetId);
                if (user == null)
                    return "Unknown User";

                var full = $"{user.FirstName?.Trim()} {user.LastName?.Trim()}".Trim();
                return string.IsNullOrWhiteSpace(full) ? "Unknown User" : full;
            }

            // --- COMMENT ---
            if (report.TargetType == ReportObjectType.Comment)
            {
                var cmt = await _commentRepo.GetByIdAsync(report.TargetId);
                var text = cmt?.Content?.Trim();
                return string.IsNullOrWhiteSpace(text) ? "(no content)" : text;
            }

            // --- RATING ---
            if (report.TargetType == ReportObjectType.Rating)
            {
                var rating = await _ratingRepo.GetByIdAsync(report.TargetId);

                if (rating == null)
                    return "Unknown Rating";

                if (string.IsNullOrWhiteSpace(rating.Feedback))
                    return $"{rating.Score} stars";

                return $"{rating.Score} stars — {rating.Feedback}";
            }

            return "Unknown";
        }

        private async Task HandleApproveAction(Report report)
        {
            // COMMENT → xóa
            if (report.TargetType == ReportObjectType.Comment)
            {
                var cmt = await _commentRepo.GetByIdAsync(report.TargetId);
                if (cmt != null)
                    await _commentRepo.DeleteAsync(cmt);
            }

            // RATING → xoá
            if (report.TargetType == ReportObjectType.Rating)
            {
                var rating = await _ratingRepo.GetByIdAsync(report.TargetId);
                if (rating != null)
                    await _ratingRepo.DeleteAsync(rating);
            }

            // USER → khoá user
            if (report.TargetType == ReportObjectType.User)
            {
                var user = await _userRepo.GetByIdAsync(report.TargetId);
                if (user != null)
                {
                    user.LockReason = "Tài khoản bị report và admin đã phê duyệt";
                    await _userRepo.UpdateAsync(user);
                }
            }

            // RECIPE → ẩn recipe
            if (report.TargetType == ReportObjectType.Recipe)
            {
                var recipe = await _recipeRepo.GetByIdAsync(report.TargetId);
                if (recipe != null)
                {
                    recipe.IsDeleted = true;
                    await _recipeRepo.UpdateAsync(recipe);
                }
            }
        }

        private async Task<IReadOnlyList<ReportResponse>> ConvertToListAsync(IList<Report> reports)
        {
            var list = new List<ReportResponse>();

            foreach (var report in reports)
            {
                var dto = _mapper.Map<ReportResponse>(report);
                dto.TargetName = await ResolveTargetName(report);
                list.Add(dto);
            }

            return list;
        }
    }
}
