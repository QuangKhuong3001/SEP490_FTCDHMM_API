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
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;
        private readonly IMapper _mapper;

        public ReportController(IReportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportRequest request)
        {

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.ReportRequest>(request);

            await _service.CreateReportAsync(userId, appRequest);

            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Report_View)]
        [HttpGet("details/{targetId:guid}")]
        public async Task<IActionResult> GetDetailList(Guid targetId, [FromQuery] string targetType)
        {
            var result = await _service.GetReportDetailsAsync(targetId, targetType);
            return Ok(result);
        }


        [Authorize(Policy = PermissionPolicies.Report_View)]
        [HttpGet]

        public async Task<IActionResult> GetList([FromQuery] ReportFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.ReportFilterRequest>(request);
            var result = await _service.GetReportsAsync(appRequest);
            return Ok(result);
        }
        [Authorize(Policy = PermissionPolicies.Report_View)]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] ReportFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.ReportFilterRequest>(request);
            var result = await _service.GetReportHistoriesAsync(appRequest);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.Report_Approve)]
        [HttpPost("{id:guid}/approve")]

        public async Task<IActionResult> Approve(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.ApproveReportAsync(id, userId);

            return Ok();
        }
        [Authorize(Policy = PermissionPolicies.Report_Reject)]
        [HttpPost("{id:guid}/reject")]

        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectReportRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.ReportDtos.RejectReportRequest>(request);

            await _service.RejectReportAsync(id, userId, appRequest.Reason);

            return Ok();
        }


    }
}
