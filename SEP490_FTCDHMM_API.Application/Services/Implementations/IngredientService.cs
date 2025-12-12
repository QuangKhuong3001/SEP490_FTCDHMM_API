using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.Nutrient;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
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
        private readonly IS3ImageService _s3ImageService;
        private readonly IImageRepository _imageRepository;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsdaApiService _usdaApi;
        private readonly IIngredientNutritionCalculator _ingredientNutritionCalculator;
        private readonly ITranslateService _translateService;
        //private readonly ICacheService _cache;

        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);
        private const int MinLength = 2;

        public IngredientService(IIngredientRepository ingredientRepository,
            IIngredientCategoryRepository ingredientCategoryRepository,
            IS3ImageService s3ImageService,
            IImageRepository imageRepository,
            IIngredientNutritionCalculator ingredientNutritionCalculator,
            ITranslateService translateService,
            INutrientRepository nutrientRepository,
            IUsdaApiService usdaApi,
            IMapper mapper, IUnitOfWork unitOfWork)
        //ICacheService cache)
        {
            _ingredientRepository = ingredientRepository;
            _ingredientCategoryRepository = ingredientCategoryRepository;
            _s3ImageService = s3ImageService;
            _nutrientRepository = nutrientRepository;
            _imageRepository = imageRepository;
            _translateService = translateService;
            _usdaApi = usdaApi;
            _ingredientNutritionCalculator = ingredientNutritionCalculator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            //_cache = cache;
        }

        private async Task HasMacroNutrients(List<Guid> nutrientIds)
        {
            var requiredNutrients = await _nutrientRepository.GetAllAsync(n => n.IsMacroNutrient);
            var requiredNutrientIds = requiredNutrients.Select(r => r.Id);

            var missingRequired = requiredNutrientIds.Except(nutrientIds).ToList();
            if (missingRequired.Any())
                throw new AppException(AppResponseCode.MISSING_REQUIRED_NUTRIENTS);
        }

        public async Task<PagedResult<IngredientResponse>> GetIngredientsAsync(IngredientFilterRequest dto)
        {
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                var exists = await _ingredientCategoryRepository.IdsExistAsync(dto.CategoryIds);
                if (!exists)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nhóm nguyên liệu không tồn tại");
            }

            dto.Keyword = dto.Keyword?.NormalizeVi();

            Expression<Func<Ingredient, bool>>? filter = null;

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                filter = i =>
                    (dto.CategoryIds == null || dto.CategoryIds.Count == 0
                    || i.Categories.Any(c => dto.CategoryIds.Contains(c.Id)));
            }

            var (ingredients, totalCount) = await _ingredientRepository.GetPagedAsync(
                pageNumber: dto.PaginationParams.PageNumber,
                pageSize: dto.PaginationParams.PageSize,
                filter: filter,
                orderBy: q => q.OrderByKeyword(dto.Keyword,
                                                        i => i.Name,
                                                        i => i.Description),

                keyword: dto.Keyword,
                searchProperties: new[] { "NormalizedName", "Description" },
                include: q => q
                    .Include(i => i.Categories)
                    .Include(i => i.Image)
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

        public async Task<PagedResult<IngredientResponse>> GetIngredientsForManagerAsync(IngredientFilterRequest dto)
        {
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                var exists = await _ingredientCategoryRepository.IdsExistAsync(dto.CategoryIds);
                if (!exists)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nhóm nguyên liệu không tồn tại");
            }

            dto.Keyword = dto.Keyword?.NormalizeVi();

            Expression<Func<Ingredient, bool>>? filter = null;

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                filter = i =>
                    (dto.CategoryIds == null || dto.CategoryIds.Count == 0
                    || i.Categories.Any(c => dto.CategoryIds.Contains(c.Id)));
            }

            var (ingredients, totalCount) = await _ingredientRepository.GetPagedAsync(
                pageNumber: dto.PaginationParams.PageNumber,
                pageSize: dto.PaginationParams.PageSize,
                filter: filter,
                orderBy: q =>
                {
                    var ordered = q.OrderByDescending(i => i.IsNew);

                    if (!string.IsNullOrWhiteSpace(dto.Keyword))
                    {
                        ordered = ordered.ThenByKeyword(dto.Keyword,
                                                        i => i.Name,
                                                        i => i.Description);
                    }

                    return ordered;
                },
                keyword: dto.Keyword,
                searchProperties: new[] { "NormalizedName", "Description" },
                include: q => q
                    .Include(i => i.Categories)
                    .Include(i => i.Image)
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

        public async Task CreateIngredientAsync(CreateIngredientRequest dto)
        {
            var upperName = dto.Name.UpperName();

            if (await _ingredientRepository.ExistsAsync(
                    i => i.UpperName == upperName))
            {
                throw new AppException(AppResponseCode.EXISTS);
            }

            if (!await _ingredientCategoryRepository.IdsExistAsync(dto.IngredientCategoryIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();
            if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            await HasMacroNutrients(nutrientIds);

            var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Image!, StorageFolder.INGREDIENTS);

            var categories = await _ingredientCategoryRepository
                .GetAllAsync(c => dto.IngredientCategoryIds.Contains(c.Id));

            if (categories.Any(c => c.IsDeleted))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var ingredient = new Ingredient
            {
                Name = dto.Name.CleanDuplicateSpace(),
                NormalizedName = dto.Name.NormalizeVi(),
                UpperName = upperName,
                Description = dto.Description == null ? DefaultValues.DEFAULT_INGREDIENT_DESCRIPTION : dto.Description,
                Image = uploadedImage,
                LastUpdatedUtc = DateTime.UtcNow,
                Categories = categories.ToList()
            };

            ingredient.IngredientNutrients = dto.Nutrients
                .Select(n => new IngredientNutrient
                {
                    IngredientId = ingredient.Id,
                    NutrientId = n.NutrientId,
                    Value = n.Value
                })
                .ToList();

            ingredient.Calories = _ingredientNutritionCalculator.CalculateCalories(
                dto.Nutrients.Select(n => new NutrientValueInput
                {
                    NutrientId = n.NutrientId,
                    Median = n.Value
                })
            );

            await _ingredientRepository.AddAsync(ingredient);
        }

        public async Task DeleteIngredientAsync(Guid ingredientId)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(
                ingredientId,
                include: i => i.Include(i => i.RecipeIngredients)
            );

            if (ingredient == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            bool isUsedInRecipe = ingredient.RecipeIngredients.Any();

            if (isUsedInRecipe)
                throw new AppException(AppResponseCode.INVALID_ACTION,
                    $"Không thể xóa nguyên liệu '{ingredient.Name}' vì nó đang được sử dụng trong một hoặc nhiều công thức.");

            if (ingredient.Image != null)
            {
                await _s3ImageService.DeleteImageAsync(ingredient.ImageId);
            }
            await _ingredientRepository.DeleteAsync(ingredient);
        }

        public async Task UpdateIngredientAsync(Guid ingredientId, UpdateIngredientRequest dto)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId,
                    include: i => i.Include(u => u.IngredientNutrients)
                                    .Include(u => u.Categories));

                if (ingredient == null)
                    throw new AppException(AppResponseCode.NOT_FOUND);

                var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();
                if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                    throw new AppException(AppResponseCode.NOT_FOUND);

                await HasMacroNutrients(nutrientIds);

                ingredient.LastUpdatedUtc = DateTime.UtcNow;

                if (!await _ingredientCategoryRepository.IdsExistAsync(dto.IngredientCategoryIds))
                    throw new AppException(AppResponseCode.NOT_FOUND);

                var categories = await _ingredientCategoryRepository
                .GetAllAsync(c => dto.IngredientCategoryIds.Contains(c.Id));

                if (categories.Any(c => c.IsDeleted))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                ingredient.Categories.Clear();
                foreach (var category in categories)
                {
                    ingredient.Categories.Add(category);
                }

                ingredient.Description = dto.Description == null ? DefaultValues.DEFAULT_INGREDIENT_DESCRIPTION : dto.Description;

                if (dto.Image != null && dto.Image.Length > 0)
                {
                    var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Image, StorageFolder.INGREDIENTS);
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
                        existingEntity.Value = nutrientDto.Value;
                    }
                    else
                    {
                        ingredient.IngredientNutrients.Add(new IngredientNutrient
                        {
                            IngredientId = ingredient.Id,
                            NutrientId = nutrientDto.NutrientId,
                            Value = nutrientDto.Value
                        });
                    }
                }

                ingredient.Calories = _ingredientNutritionCalculator.CalculateCalories(
                    dto.Nutrients.Select(n => new NutrientValueInput
                    {
                        NutrientId = n.NutrientId,
                        Median = n.Value
                    })
                );

                await _ingredientRepository.UpdateAsync(ingredient);
            });
        }
        public async Task<IngredientDetailsResponse> GetIngredientDetailsAsync(Guid ingredientId)
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

            return result;
        }
        public async Task<IngredientDetailsResponse> GetIngredientDetailsForManagerAsync(Guid ingredientId)
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

            ingredient.IsNew = false;
            await _ingredientRepository.UpdateAsync(ingredient);

            var result = _mapper.Map<IngredientDetailsResponse>(ingredient);

            return result;
        }

        public async Task<IEnumerable<IngredientNameResponse>> GetFromUsdaSourceAsync(string keyword)
        {
            keyword = keyword.CleanDuplicateSpace().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < MinLength)
                return Enumerable.Empty<IngredientNameResponse>();

            var dbItems = await _ingredientRepository.GetTop5Async(keyword);
            if (dbItems.Count > 0)
                return _mapper.Map<List<IngredientNameResponse>>(dbItems);

            var translated = await _translateService.TranslateToEnglishAsync(keyword);

            var created = await CreateIngredientFromUsdaAsync(translated);
            if (created == null)
                return Enumerable.Empty<IngredientNameResponse>();

            return new List<IngredientNameResponse> { created };
        }

        private async Task<IngredientNameResponse?> CreateIngredientFromUsdaAsync(string translated)
        {
            var vietName = await _translateService.TranslateToVietnameseAsync(translated);

            var upperName = vietName.UpperName();
            if (await _ingredientRepository.ExistsAsync(u => u.UpperName == upperName))
            {
                var dbItems = await _ingredientRepository.GetTop5Async(upperName);
                return _mapper.Map<IngredientNameResponse>(dbItems.First());
            }

            var searchResults = await _usdaApi.SearchAsync(translated);
            if (searchResults == null)
                return null;

            var detail = await _usdaApi.GetDetailAsync(searchResults.FdcId);
            if (detail == null)
                return null;

            var systemNutrients = await _nutrientRepository.GetAllAsync();

            var defaultImage = await _imageRepository.GetDefaultImageAsync();

            const string USDA_CATEGORY_NAME = "USDA Imported";
            var category = await _ingredientCategoryRepository.FirstOrDefaultAsync(u => u.Name == USDA_CATEGORY_NAME);
            if (category == null)
            {
                category = new IngredientCategory
                {
                    Name = USDA_CATEGORY_NAME
                };
                await _ingredientCategoryRepository.AddAsync(category);
            }

            var descriptionViet = await _translateService.TranslateToVietnameseAsync(detail.Description);

            var ingredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = vietName,
                NormalizedName = vietName.NormalizeVi(),
                UpperName = upperName,
                Description = descriptionViet,
                LastUpdatedUtc = DateTime.UtcNow,
                Calories = ExtractCalories(detail),
                ImageId = defaultImage.Id,
                Image = defaultImage,
                IsNew = true
            };

            ingredient.Categories.Add(category);
            ingredient.IngredientNutrients = MapNutrients(detail, ingredient, systemNutrients);

            await _ingredientRepository.AddAsync(ingredient);

            return new IngredientNameResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name
            };
        }

        private static decimal ExtractCalories(UsdaFoodDetail detail)
        {
            var kcal = detail.FoodNutrients
                .FirstOrDefault(n =>
                    n.Nutrient.Name.Contains("energy", StringComparison.OrdinalIgnoreCase) &&
                    n.Nutrient.UnitName.Equals("kcal", StringComparison.OrdinalIgnoreCase))
                ?.Amount;

            return kcal.HasValue ? (decimal)kcal.Value : 0m;
        }

        private static List<IngredientNutrient> MapNutrients(
            UsdaFoodDetail detail,
            Ingredient ingredient,
            IList<Nutrient> systemNutrients)
        {
            var list = new List<IngredientNutrient>();

            foreach (var usda in detail.FoodNutrients)
            {
                var usdaNameNorm =
                    ViStringExtensions.RemoveDiacritics(usda.Nutrient.Name.ToLower());

                var sys = systemNutrients.FirstOrDefault(n =>
                    ViStringExtensions.RemoveDiacritics(n.Name.ToLower())
                        .Equals(usdaNameNorm, StringComparison.OrdinalIgnoreCase));

                if (sys == null)
                    continue;

                list.Add(new IngredientNutrient
                {
                    IngredientId = ingredient.Id,
                    Ingredient = ingredient,
                    NutrientId = sys.Id,
                    Nutrient = sys,
                    Value = usda.Amount ?? 0m,
                });
            }

            return list;
        }
    }
}
