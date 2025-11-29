using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;
        private readonly IMapper _mapper;

        public ReportController(IReportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // =====================================
        // 🔥 HELPER: Lấy UserId từ JWT token
        // =====================================
        private Guid GetUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(val))
                throw new AppException(AppResponseCode.UNAUTHORIZED, "Bạn chưa đăng nhập.");

            return Guid.Parse(val);
        }

        // =====================================
        // ⭐ 1. CREATE REPORT
        // =====================================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReportRequest request)
        {
            var reporterId = GetUserId();
            var appRequest = _mapper.Map<ReportRequest>(request);

            await _service.CreateReportAsync(reporterId, appRequest);

            return Ok(new { message = "Report created successfully." });
        }

        // =====================================
        // ⭐ 2. GET ALL PENDING (ADMIN)
        // =====================================
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetAllPendingAsync();
            return Ok(result);
        }

        // =====================================
        // ⭐ 3. GET BY TYPE (ADMIN)
        // =====================================
        [HttpGet("type/{type}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByType(string type)
        {
            var result = await _service.GetReportsByTypeAsync(type);
            return Ok(result);
        }

        // =====================================
        // ⭐ 4. GET REPORT BY ID (ADMIN)
        // =====================================
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        // =====================================
        // ⭐ 5. APPROVE REPORT (ADMIN)
        // =====================================
        [HttpPost("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var adminId = GetUserId();
            await _service.ApproveAsync(id, adminId);

            return Ok(new { message = "Report approved." });
        }

        // =====================================
        // ⭐ 6. REJECT REPORT (ADMIN)
        // =====================================
        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var adminId = GetUserId();
            await _service.RejectAsync(id, adminId);

            return Ok(new { message = "Report rejected." });
        }

        // =====================================
        // ⭐ 7. DELETE REPORT (ADMIN)
        // =====================================
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);

            return Ok(new { message = "Report deleted." });
        }
    }
}
