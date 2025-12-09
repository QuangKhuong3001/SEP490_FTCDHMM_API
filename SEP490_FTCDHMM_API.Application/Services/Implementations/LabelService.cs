using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

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

        public async Task CreatLabelAsync(CreateLabelRequest dto)
        {
            var lowerName = dto.Name.ToLowerInvariant().CleanDuplicateSpace();
            var normalizeName = dto.Name.NormalizeVi();

            if (await _labelRepository.ExistsAsync(l => l.LowerName == lowerName))
                throw new AppException(AppResponseCode.EXISTS);

            await _labelRepository.AddAsync(new Label
            {
                Name = dto.Name.CleanDuplicateSpace(),
                LowerName = lowerName,
                NormalizedName = normalizeName,
                ColorCode = dto.ColorCode
            });
        }
        public async Task<PagedResult<LabelResponse>> GetPagedLabelsAsync(LabelFilterRequest request)
        {
            var normalizeKeyword = request.Keyword?.NormalizeVi();

            var (labels, totalCount) = await _labelRepository.GetPagedAsync(
                request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                l => l.IsDeleted == false &&
                    (string.IsNullOrEmpty(request.Keyword) || l.NormalizedName.Contains(normalizeKeyword!)),
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
        public async Task<IEnumerable<LabelResponse>> GetLabelsAsync(LabelSearchDropboxRequest request)
        {
            var normalizeKeyword = request.Keyword?.NormalizeVi();

            var labels = await _labelRepository.GetAllAsync(
                l => !l.IsDeleted &&
                    (string.IsNullOrEmpty(request.Keyword) || l.NormalizedName.Contains(normalizeKeyword!)));
            labels = labels.OrderBy(l => l.Name).ToList();

            var result = _mapper.Map<IEnumerable<LabelResponse>>(labels);
            return result;
        }

        public async Task DeleteLabelAsync(Guid labelId)
        {
            var label = await _labelRepository.GetByIdAsync(labelId,
                include: l => l.Include(u => u.Recipes));
            if (label == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (label.Recipes.Any())
            {
                label.IsDeleted = true;
                await _labelRepository.UpdateAsync(label);
            }
            else
            {
                await _labelRepository.DeleteAsync(label);
            }
        }

        public async Task UpdateColorCodeAsync(Guid labelId, UpdateColorCodeRequest request)
        {
            var label = await _labelRepository.GetByIdAsync(labelId);

            if (label == null || label.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            label.ColorCode = request.ColorCode;
            await _labelRepository.UpdateAsync(label);
        }
    }
}
