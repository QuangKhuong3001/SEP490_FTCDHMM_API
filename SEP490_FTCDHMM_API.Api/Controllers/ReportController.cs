using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.ReportDtos;

using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
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
        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReportRequest request)
        {
            var userId = GetUserId();

            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.ReportRequest>(request);

            await _service.CreateAsync(userId, appRequest);

            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Report_View)]
        [HttpGet("{id:guid}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        [Authorize(Policy = PermissionPolicies.Report_View)]
        [HttpGet("summary")]

        public async Task<IActionResult> GetSummary([FromQuery] ReportFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.ReportFilterRequest>(request);
            var result = await _service.GetSummaryAsync(appRequest);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.Report_Approve)]
        [HttpPost("{id:guid}/approve")]

        public async Task<IActionResult> Approve(Guid id)
        {
            var adminId = GetUserId();
            await _service.ApproveAsync(id, adminId);

            return Ok();
        }
        [Authorize(Policy = PermissionPolicies.Report_Reject)]
        [HttpPost("{id:guid}/reject")]

        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectReportRequest request)
        {
            var adminId = GetUserId();

            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.RejectReportRequest>(request);

            await _service.RejectAsync(id, adminId, appRequest.Reason);

            return Ok();
        }


    }
}
