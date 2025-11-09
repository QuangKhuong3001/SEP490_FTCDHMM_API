using AutoMapper;
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

        public async Task CreateCategory(CreateIngredientCategoryRequest request)
        {
            var exits = await _ingredientCategoryRepository.ExistsAsync(c => c.Name == request.Name);
            if (exits)
                throw new AppException(AppResponseCode.EXISTS);

            await _ingredientCategoryRepository.AddAsync(new IngredientCategory { Name = request.Name });
        }
        public async Task DeleteCategory(Guid id)
        {
            var category = await _ingredientCategoryRepository.GetByIdAsync(id, ic => ic.Ingredients);
            if (category == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (category.Ingredients.Any())
            {
                category.isDeleted = true;
            }
            else
            {
                await _ingredientCategoryRepository.DeleteAsync(category);
            }
        }

        public async Task<PagedResult<IngredientCategoryResponse>> GetAllCategories(IngredientCategoryFilterRequest request)
        {
            var (categories, totalCount) = await _ingredientCategoryRepository.GetPagedAsync(
                            request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                            l => l.isDeleted == false &&
                                (string.IsNullOrEmpty(request.Keyword) || l.Name.Contains(request.Keyword!)),
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
        public async Task<IEnumerable<IngredientCategoryResponse>> GetAllCategories(IngredientCategorySearchDropboxRequest request)
        {
            var ingredients = await _ingredientCategoryRepository.GetAllAsync(
                            l => !l.isDeleted &&
                                (string.IsNullOrEmpty(request.Keyword) || l.Name.Contains(request.Keyword!)));
            ingredients = ingredients.OrderBy(l => l.Name).ToList();

            var result = _mapper.Map<IEnumerable<IngredientCategoryResponse>>(ingredients);
            return result;
        }
    }
}
