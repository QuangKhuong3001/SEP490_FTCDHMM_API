using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;
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
        private readonly IS3ImageService _s3ImageService;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);
        private const int MinLenth = 2;

        public IngredientService(
            IIngredientRepository ingredientRepository,
            IIngredientCategoryRepository ingredientCategoryRepository,
            INutrientRepository nutrientRepository,
            IS3ImageService s3ImageService,
            IUnitOfWork unitOfWork,
            ICacheService cache,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _ingredientCategoryRepository = ingredientCategoryRepository;
            _nutrientRepository = nutrientRepository;
            _s3ImageService = s3ImageService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResult<IngredientResponse>> GetList(IngredientFilterRequest dto)
        {
            Expression<Func<Ingredient, bool>>? filter = null;

            if ((dto.CategoryIds != null && dto.CategoryIds.Any()) ||
                dto.UpdatedFrom.HasValue || dto.UpdatedTo.HasValue)
            {
                filter = i =>
                    ((dto.CategoryIds == null || !dto.CategoryIds.Any()) ||
                        i.IngredientCategoryAssignments.Any(a => dto.CategoryIds.Contains(a.CategoryId))) &&

                    (!dto.UpdatedFrom.HasValue || i.LastUpdatedUtc >= dto.UpdatedFrom) &&
                    (!dto.UpdatedTo.HasValue || i.LastUpdatedUtc <= dto.UpdatedTo);
            }


            var (ingredients, totalCount) = await _ingredientRepository.GetPagedAsync(
                page: dto.PageNumber,
                pageSize: dto.PageSize,
                filter: filter,
                orderBy: q => q.OrderByKeyword(dto.Keyword,
                                           i => i.Name,
                                           i => i.Description),
                keyword: dto.Keyword,
                searchProperties: new[] { "Name", "Description" },
                include: q => q
                    .Include(i => i.IngredientCategoryAssignments)
                        .ThenInclude(a => a.Category)
            );

            var result = _mapper.Map<List<IngredientResponse>>(ingredients);

            return new PagedResult<IngredientResponse>
            {
                Items = result,
                TotalCount = totalCount,
                Page = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        public async Task CreateIngredient(CreateIngredientRequest dto)
        {
            if (await _ingredientRepository.ExistsAsync(i => i.Name == dto.Name))
                throw new AppException(AppResponseCode.NAME_ALREADY_EXISTS);

            if (!await _ingredientCategoryRepository.IdsExistAsync(dto.IngredientCategoryIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();
            if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var requiredNames = new[] { "Calories", "Protein", "Fat", "Carbohydrate" };
            var allNutrients = await _nutrientRepository.GetAllAsync();
            var requiredIds = allNutrients
                .Where(n => requiredNames.Contains(n.Name))
                .Select(n => n.Id)
                .ToList();

            var missingRequired = requiredIds.Except(nutrientIds).ToList();
            if (missingRequired.Any())
                throw new AppException(AppResponseCode.MISSING_REQUIRED_NUTRIENTS);

            var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Image, StorageFolder.INGREDIENTS, null);

            var ingredient = new Ingredient
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Image = uploadedImage,
                LastUpdatedUtc = DateTime.UtcNow
            };

            ingredient.IngredientCategoryAssignments = dto.IngredientCategoryIds
                .Select(id => new IngredientCategoryAssignment { CategoryId = id, Ingredient = ingredient })
                .ToList();

            ingredient.IngredientNutrients = dto.Nutrients
                .Select(n => new IngredientNutrient
                {
                    Ingredient = ingredient,
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

            //Miss Content  var used = wait to develop recipe module, its user can delete which one no use 

            await _ingredientRepository.DeleteAsync(ingredient);
        }
        public async Task UpdateIngredient(Guid ingredientId, UpdateIngredientRequest dto, CancellationToken ct)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(
                ingredientId, i => i.IngredientNutrients);

                if (ingredient == null)
                    throw new AppException(AppResponseCode.NOT_FOUND);

                ingredient.LastUpdatedUtc = DateTime.UtcNow;

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

                if (dto.Nutrients != null)
                {
                    var allNutrients = await _nutrientRepository.GetAllAsync();
                    var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();

                    if (!await _nutrientRepository.IdsExistAsync(nutrientIds))
                        throw new AppException(AppResponseCode.NOT_FOUND);

                    var existing = ingredient.IngredientNutrients.ToList();

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
                    .Include(i => i.IngredientCategoryAssignments)
                        .ThenInclude(a => a.Category)
            );

            if (ingredient == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var ImageUrl = _s3ImageService.GeneratePreSignedUrl(ingredient.Image.Key);

            var result = _mapper.Map<IngredientDetailsResponse>(ingredient);
            result.ImageUrl = ImageUrl!;

            return result;
        }

        public async Task<List<IngredientNameResponse>> GetTop5Async(string keyword, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Trim().Length < MinLenth)
                return new();

            keyword = keyword.Trim();
            var key = $"ingredient:search:{keyword.ToLowerInvariant()}";

            var cached = await _cache.GetAsync<List<IngredientNameResponse>>(key, ct);
            if (cached is { Count: > 0 }) return cached;

            var dbItems = await _ingredientRepository.GetTop5Async(keyword, ct);

            var mapped = _mapper.Map<List<IngredientNameResponse>>(dbItems);

            if (mapped.Count > 0)
            {
                await _cache.SetAsync(key, mapped, _ttl, ct);
                return mapped;
            }

            return new();
        }
    }
}

