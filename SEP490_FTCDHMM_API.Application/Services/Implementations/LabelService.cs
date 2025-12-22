using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
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
        private readonly ICacheService _cache;


        public LabelService(
            ILabelRepository labelRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _labelRepository = labelRepository;
            _mapper = mapper;
            _cache = cacheService;
        }

        public async Task CreatLabelAsync(CreateLabelRequest dto)
        {
            var upperName = dto.Name.UpperName();
            var normalizeName = dto.Name.NormalizeVi();

            if (await _labelRepository.ExistsAsync(l => l.UpperName == upperName))
                throw new AppException(AppResponseCode.EXISTS);

            await _labelRepository.AddAsync(new Label
            {
                Name = dto.Name.CleanDuplicateSpace(),
                UpperName = upperName,
                NormalizedName = normalizeName,
                ColorCode = dto.ColorCode
            });

            await _cache.RemoveByPrefixAsync("label");

        }
        public async Task<PagedResult<LabelResponse>> GetPagedLabelsAsync(LabelFilterRequest request)
        {
            var normalizeKeyword = request.Keyword?.NormalizeVi();

            var cacheKey = $"label:paged:{normalizeKeyword}";

            var cached = await _cache.GetAsync<PagedResult<LabelResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var (labels, totalCount) = await _labelRepository.GetPagedAsync(
                request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                l => string.IsNullOrEmpty(request.Keyword) || l.NormalizedName.Contains(normalizeKeyword!),
                q => q.OrderBy(u => u.Name));

            var result = _mapper.Map<List<LabelResponse>>(labels);

            var resultPaged = new PagedResult<LabelResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };

            await _cache.SetAsync(cacheKey, resultPaged, TimeSpan.FromMinutes(30));

            return resultPaged;
        }
        public async Task<IEnumerable<LabelResponse>> GetLabelsAsync(LabelSearchDropboxRequest request)
        {
            var normalizeKeyword = request.Keyword?.NormalizeVi();
            var cacheKey = $"label:dropdown:{normalizeKeyword}";

            var cached = await _cache.GetAsync<IEnumerable<LabelResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var labels = await _labelRepository.GetAllAsync(
                l => string.IsNullOrEmpty(request.Keyword) || l.NormalizedName.Contains(normalizeKeyword!));
            labels = labels.OrderBy(l => l.Name).ToList();

            var result = _mapper.Map<IEnumerable<LabelResponse>>(labels);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
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
                throw new AppException(AppResponseCode.INVALID_ACTION, "Nhãn đang được sử dụng, không thể xóa");
            }
            else
            {
                await _labelRepository.DeleteAsync(label);
            }

            await _cache.RemoveByPrefixAsync("label");
        }

        public async Task UpdateColorCodeAsync(Guid labelId, UpdateColorCodeRequest request)
        {
            var label = await _labelRepository.GetByIdAsync(labelId);

            if (label == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (label.LastUpdatedUtc != request.LastUpdatedUtc)
                throw new AppException(AppResponseCode.CONFLICT);

            label.LastUpdatedUtc = DateTime.UtcNow;
            label.ColorCode = request.ColorCode;
            await _labelRepository.UpdateAsync(label);
        }
    }
}
