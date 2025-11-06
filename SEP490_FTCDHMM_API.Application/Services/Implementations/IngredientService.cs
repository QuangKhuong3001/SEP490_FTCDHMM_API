using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;


namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IS3ImageService _s3ImageService;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ICacheService _cache;

        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);
        private const int MinLenth = 2;

        public IngredientService(IIngredientRepository ingredientRepository,
            IIngredientCategoryRepository ingredientCategoryRepository,
            IRecipeRepository recipeRepository,
            IS3ImageService s3ImageService,
            INutrientRepository nutrientRepository,
            IMapper mapper, IUnitOfWork unitOfWork)
        //ICacheService cache)
        {
            _ingredientRepository = ingredientRepository;
            _ingredientCategoryRepository = ingredientCategoryRepository;
            _recipeRepository = recipeRepository;
            _s3ImageService = s3ImageService;
            _nutrientRepository = nutrientRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            //_cache = cache;
        }

        private async Task HasNutrientsRequied(List<Guid> nutrientIds)
        {
            var requiredNutrients = await _nutrientRepository.GetAllAsync(n => n.IsRequired);
            var requiredNutrientIds = requiredNutrients.Select(r => r.Id);

            var missingRequired = requiredNutrientIds.Except(nutrientIds).ToList();
            if (missingRequired.Any())
                throw new AppException(AppResponseCode.MISSING_REQUIRED_NUTRIENTS);
        }

        private static void ValidateValueInput(NutrientRequest n)
        {
            if ((n.Min > n.Max) || (n.Min > n.Median) || (n.Median > n.Max))
                throw new AppException(AppResponseCode.INVALID_ACTION);
        }

        public async Task<PagedResult<IngredientResponse>> GetList(IngredientFilterRequest dto)
        {
            Expression<Func<Ingredient, bool>>? filter = null;

            if ((dto.CategoryIds != null && dto.CategoryIds.Any()) ||
                dto.UpdatedFrom.HasValue || dto.UpdatedTo.HasValue)
            {
                filter = i =>
                    ((dto.CategoryIds == null || !dto.CategoryIds.Any()) ||
                    i.Categories.Any(c => dto.CategoryIds.Contains(c.Id))) &&
                    (!dto.UpdatedFrom.HasValue || i.LastUpdatedUtc >= dto.UpdatedFrom) &&
                    (!dto.UpdatedTo.HasValue || i.LastUpdatedUtc <= dto.UpdatedTo);
            }


            var (ingredients, totalCount) = await _ingredientRepository.GetPagedAsync(
                pageNumber: dto.PaginationParams.PageNumber,
                pageSize: dto.PaginationParams.PageSize,
                filter: filter,
                orderBy: q => q.OrderByKeyword(dto.Keyword,
                                           i => i.Name,
                                           i => i.Description),
                keyword: dto.Keyword,
                searchProperties: new[] { "Name", "Description" },
                include: q => q
                    .Include(i => i.Categories)
            );

            var result = _mapper.Map<List<IngredientResponse>>(ingredients);

            return new PagedResult<IngredientResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = dto.PaginationParams.PageNumber,
                PageSize = dto.PaginationParams.PageSize
            };
        }

        public async Task CreateIngredient(CreateIngredientRequest dto)
        {
            if (await _ingredientRepository.ExistsAsync(i => i.Name == dto.Name))
                throw new AppException(AppResponseCode.NAME_ALREADY_EXISTS);

            if (!await _ingredientCategoryRepository.IdsExistAsync(dto.IngredientCategoryIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            foreach (var n in dto.Nutrients)
            {
                IngredientService.ValidateValueInput(n);
            }

            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();
            if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            await HasNutrientsRequied(nutrientIds);

            var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Image, StorageFolder.INGREDIENTS, null);

            var categories = await _ingredientCategoryRepository
                .GetAllAsync(c => dto.IngredientCategoryIds.Contains(c.Id));

            if (categories.Any(c => c.isDeleted))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var ingredient = new Ingredient
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Image = uploadedImage,
                LastUpdatedUtc = DateTime.UtcNow,
                Categories = categories.ToList()
            };

            ingredient.IngredientNutrients = dto.Nutrients
                .Select(n => new IngredientNutrient
                {
                    IngredientId = ingredient.Id,
                    NutrientId = n.NutrientId,
                    Min = n.Min,
                    Max = n.Max,
                    Median = n.Median
                })
                .ToList();

            await _ingredientRepository.AddAsync(ingredient);
        }
        public async Task DeleteIngredient(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);

            if (ingredient == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (await _recipeRepository.ExistsAsync(c => c.Ingredients.Contains(ingredient)))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _ingredientRepository.DeleteAsync(ingredient);
        }
        public async Task UpdateIngredient(Guid ingredientId, UpdateIngredientRequest dto, CancellationToken ct)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(
                ingredientId, i => i.IngredientNutrients, i => i.Categories);

                if (ingredient == null)
                    throw new AppException(AppResponseCode.NOT_FOUND);

                var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();
                if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                    throw new AppException(AppResponseCode.NOT_FOUND);

                await HasNutrientsRequied(nutrientIds);

                foreach (var n in dto.Nutrients)
                {
                    IngredientService.ValidateValueInput(n);
                }

                ingredient.LastUpdatedUtc = DateTime.UtcNow;

                var categories = await _ingredientCategoryRepository
                .GetAllAsync(c => dto.IngredientCategoryIds.Contains(c.Id));

                if (categories.Any(c => c.isDeleted))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                ingredient.Categories.Clear();
                foreach (var category in categories)
                {
                    ingredient.Categories.Add(category);
                }

                if (!string.IsNullOrEmpty(dto.Description))
                    ingredient.Description = dto.Description.Trim();

                if (dto.Image != null && dto.Image.Length > 0)
                {
                    var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Image, StorageFolder.INGREDIENTS, null);
                    var oldImageId = ingredient.ImageId;

                    ingredient.Image = uploadedImage;

                    await _ingredientRepository.UpdateAsync(ingredient);

                    _unitOfWork.RegisterAfterCommit(async () =>
                    {
                        await _s3ImageService.DeleteImageAsync(oldImageId);
                    });
                }

                var existing = ingredient.IngredientNutrients.ToList();
                var newIds = dto.Nutrients.Select(n => n.NutrientId).ToList();

                var toRemove = existing.Where(x => !newIds.Contains(x.NutrientId)).ToList();
                foreach (var removeItem in toRemove)
                {
                    ingredient.IngredientNutrients.Remove(removeItem);
                }

                foreach (var nutrientDto in dto.Nutrients)
                {
                    var existingEntity = existing.FirstOrDefault(x => x.NutrientId == nutrientDto.NutrientId);
                    if (existingEntity != null)
                    {
                        existingEntity.Min = nutrientDto.Min;
                        existingEntity.Max = nutrientDto.Max;
                        existingEntity.Median = nutrientDto.Median;
                    }
                    else
                    {
                        ingredient.IngredientNutrients.Add(new IngredientNutrient
                        {
                            IngredientId = ingredient.Id,
                            NutrientId = nutrientDto.NutrientId,
                            Min = nutrientDto.Min,
                            Max = nutrientDto.Max,
                            Median = nutrientDto.Median
                        });
                    }
                }

                await _ingredientRepository.UpdateAsync(ingredient);
            });
        }
        public async Task<IngredientDetailsResponse> GetDetails(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(
                ingredientId,
                include: q => q
                    .Include(i => i.Image)
                    .Include(i => i.IngredientNutrients)
                        .ThenInclude(n => n.Nutrient)
                            .ThenInclude(n => n.Unit)
                    .Include(i => i.Categories)
            );

            if (ingredient == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var result = _mapper.Map<IngredientDetailsResponse>(ingredient);

            // Tạo Signed URL cho hình ảnh S3 thay vì dùng URL trực tiếp
            // Vì Bucket S3 được cấu hình ở chế độ Private (không public), nên URL trực tiếp sẽ trả về 403 Forbidden
            // Signed URL cho phép truy cập tạm thời (có thời hạn) vào object private trên S3 mà không cần public bucket
            // Frontend nhận URL đã register và có thể hiển thị hình ảnh trong khoảng thời gian giới hạn (mặc định 7 ngày)
            if (ingredient.Image?.Key != null)
            {
                result.ImageUrl = _s3ImageService.GeneratePreSignedUrl(ingredient.Image.Key) ?? string.Empty;
            }

            return result;
        }

        public async Task<List<IngredientNameResponse>> GetTop5Async(string keyword, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Trim().Length < MinLenth)
                return new();

            keyword = keyword.Trim();
            var key = $"ingredient:search:{keyword.ToLowerInvariant()}";

            //var cached = await _cache.GetAsync<List<IngredientNameResponse>>(key, ct);
            //if (cached is { Count: > 0 }) return cached;

            var dbItems = await _ingredientRepository.GetTop5Async(keyword, ct);

            var mapped = _mapper.Map<List<IngredientNameResponse>>(dbItems);

            if (mapped.Count > 0)
            {
                //await _cache.SetAsync(key, mapped, _ttl, ct);
                return mapped;
            }

            return new();
        }
    }
}
