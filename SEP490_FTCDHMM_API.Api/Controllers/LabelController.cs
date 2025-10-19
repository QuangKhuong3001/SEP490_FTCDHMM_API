using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILabelService _labelService;

        public LabelController(IMapper mapper, ILabelService labelService)
        {
            _mapper = mapper;
            _labelService = labelService;
        }

        [HttpGet("getListFilter")]
        public async Task<IActionResult> GetList([FromQuery] LabelFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.LabelDtos.LabelFilterRequest>(request);

            var result = await _labelService.GetAllLabels(appRequest);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] LabelSearchDropboxRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.LabelDtos.LabelSearchDropboxRequest>(request);

            var result = await _labelService.GetAllLabels(appRequest);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.Label_Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateLabelRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.LabelDtos.CreateLabelRequest>(request);

            await _labelService.CreatLabel(appRequest);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Label_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _labelService.DeleteLabel(id);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.Label_Update)]
        [HttpPut("{id:guid}/colorCode")]
        public async Task<IActionResult> UpdaetColor(Guid id, UpdateColorCodeRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.LabelDtos.UpdateColorCodeRequest>(request);

            await _labelService.UpdateColorCode(id, appRequest);
            return Ok();
        }
    }
}
