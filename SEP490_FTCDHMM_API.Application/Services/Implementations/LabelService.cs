using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IMapper _mapper;

        public LabelService(ILabelRepository labelRepository, IMapper mapper)
        {
            _labelRepository = labelRepository;
            _mapper = mapper;
        }

        public async Task CreatLabel(CreateLabelRequest dto)
        {
            if (await _labelRepository.ExistsAsync(l => l.Name == dto.Name))
                throw new AppException(AppResponseCode.NAME_ALREADY_EXISTS);

            await _labelRepository.AddAsync(new Label
            {
                Name = dto.Name,
                ColorCode = dto.ColorCode
            });
        }
        public async Task<PagedResult<LabelResponse>> GetAllLabels(LabelFilterRequest request)
        {
            var (labels, totalCount) = await _labelRepository.GetPagedAsync(
                request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                l => l.isDeleted == false,
                q => q.OrderBy(u => u.Name));

            var result = _mapper.Map<List<LabelResponse>>(labels);

            return new PagedResult<LabelResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }
        public async Task<List<LabelResponse>> GetAllLabels(LabelSearchDropboxRequest request)
        {
            var labels = await _labelRepository.GetAllAsync(
                l => !l.isDeleted &&
                     (string.IsNullOrEmpty(request.Keyword) || l.Name.Contains(request.Keyword!)));
            labels = labels.OrderBy(l => l.Name).ToList();

            var result = _mapper.Map<List<LabelResponse>>(labels);
            return result;
        }

        public async Task DeleteLabel(Guid labelId)
        {
            var label = await _labelRepository.GetByIdAsync(labelId, l => l.Recipes);
            if (label == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (label.Recipes.Any())
            {
                label.isDeleted = true;
                await _labelRepository.UpdateAsync(label);
            }
            else
            {
                await _labelRepository.DeleteAsync(label);
            }
        }

        public async Task UpdateColorCode(Guid labelId, UpdateColorCodeRequest request)
        {
            var label = await _labelRepository.GetByIdAsync(labelId);

            if (label == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            label.ColorCode = request.ColorCode;
            await _labelRepository.UpdateAsync(label);
        }
    }
}
