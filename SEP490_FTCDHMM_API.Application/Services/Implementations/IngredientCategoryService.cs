using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class IngredientCategoryService : IIngredientCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
        private readonly ICacheService _cache;

        public IngredientCategoryService(
            IMapper mapper,
            ICacheService cache,
            IIngredientCategoryRepository ingredientCategoryRepository)
        {
            _mapper = mapper;
            _cache = cache;
            _ingredientCategoryRepository = ingredientCategoryRepository;
        }

        public async Task CreateIngredientCategoryAsync(CreateIngredientCategoryRequest request)
        {
            var upperName = request.Name.Trim().ToUpperInvariant();
            var normalizedName = request.Name.NormalizeVi();

            var exists = await _ingredientCategoryRepository.ExistsAsync(
                c => c.UpperName == upperName);

            if (exists)
                throw new AppException(AppResponseCode.EXISTS);

            await _ingredientCategoryRepository.AddAsync(
                new IngredientCategory
                {
                    Name = request.Name,
                    UpperName = upperName,
                    NormalizedName = normalizedName
                });

            await _cache.RemoveByPrefixAsync("ingredient-category");
        }
        public async Task DeleteIngredientCategoryAsync(Guid id)
        {
            var category = await _ingredientCategoryRepository.GetByIdAsync(id,
                include: i => i.Include(u => u.Ingredients));

            if (category == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (category.Ingredients.Any())
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Danh mục nguyên liệu đang được sử dụng, không thể xóa");
            }
            else
            {
                await _ingredientCategoryRepository.DeleteAsync(category);
            }
            await _cache.RemoveByPrefixAsync("ingredient-category");
        }

        public async Task<PagedResult<IngredientCategoryResponse>> GetIngredientCategoriesFilterAsync(IngredientCategoryFilterRequest request)
        {
            var normalizedKeyword = request.Keyword.NormalizeVi() ?? string.Empty;

            var (categories, totalCount) = await _ingredientCategoryRepository.GetPagedAsync(
                            request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                            l => string.IsNullOrEmpty(request.Keyword) || l.NormalizedName.Contains(normalizedKeyword),
                            q => q.OrderBy(u => u.Name));

            var result = _mapper.Map<List<IngredientCategoryResponse>>(categories);

            return new PagedResult<IngredientCategoryResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }
        public async Task<IEnumerable<IngredientCategoryResponse>> GetIngredientCategoriesAsync(IngredientCategorySearchDropboxRequest request)
        {
            var normalizedKeyword = request.Keyword.NormalizeVi() ?? string.Empty;
            var cacheKey = $"ingredient-category:dropdown:{normalizedKeyword}";

            var cached = await _cache.GetAsync<IEnumerable<IngredientCategoryResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var ingredients = await _ingredientCategoryRepository.GetAllAsync(
                            l => l.NormalizedName.Contains(normalizedKeyword));

            ingredients = ingredients.OrderBy(l => l.Name).ToList();

            var result = _mapper.Map<IEnumerable<IngredientCategoryResponse>>(ingredients);
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }
    }
}
