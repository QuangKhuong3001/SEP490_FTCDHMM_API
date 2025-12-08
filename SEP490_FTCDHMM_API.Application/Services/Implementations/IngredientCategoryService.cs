using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class IngredientCategoryService : IIngredientCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IIngredientCategoryRepository _ingredientCategoryRepository;

        public IngredientCategoryService(IMapper mapper, IIngredientCategoryRepository ingredientCategoryRepository)
        {
            _mapper = mapper;
            _ingredientCategoryRepository = ingredientCategoryRepository;
        }

        public async Task CreateIngredientCategoryAsync(CreateIngredientCategoryRequest request)
        {
            var normalizedName = request.Name.Trim().ToLower();

            var exists = await _ingredientCategoryRepository.ExistsAsync(
                c => c.Name.ToLower().Trim() == normalizedName);

            if (exists)
                throw new AppException(AppResponseCode.EXISTS);

            await _ingredientCategoryRepository.AddAsync(new IngredientCategory { Name = request.Name });
        }
        public async Task DeleteIngredientCategoryAsync(Guid id)
        {
            var category = await _ingredientCategoryRepository.GetByIdAsync(id,
                include: i => i.Include(u => u.Ingredients));

            if (category == null || category.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (category.Ingredients.Any())
            {
                category.IsDeleted = true;
                await _ingredientCategoryRepository.UpdateAsync(category);
            }
            else
            {
                await _ingredientCategoryRepository.DeleteAsync(category);
            }
        }

        public async Task<PagedResult<IngredientCategoryResponse>> GetAllIngredientCategoriesFilterAsync(IngredientCategoryFilterRequest request)
        {
            var (categories, totalCount) = await _ingredientCategoryRepository.GetPagedAsync(
                            request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                            l => l.IsDeleted == false &&
                                (string.IsNullOrEmpty(request.Keyword) || l.Name.ToLowerInvariant().Contains(request.Keyword.Trim().ToLowerInvariant()!)),
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
            var ingredients = await _ingredientCategoryRepository.GetAllAsync(
                            l => !l.IsDeleted &&
                                (string.IsNullOrEmpty(request.Keyword) || l.Name.Contains(request.Keyword!)));
            ingredients = ingredients.OrderBy(l => l.Name).Take(5).ToList();

            var result = _mapper.Map<IEnumerable<IngredientCategoryResponse>>(ingredients);
            return result;
        }
    }
}
