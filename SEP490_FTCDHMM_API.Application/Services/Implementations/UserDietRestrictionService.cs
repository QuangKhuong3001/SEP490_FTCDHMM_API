using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserDietRestrictionService : IUserDietRestrictionService
    {
        private readonly IUserDietRestrictionRepository _userDietRestrictionRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public UserDietRestrictionService(IUserDietRestrictionRepository userDietRestrictionRepository,
            IMapper mapper,
            IIngredientCategoryRepository ingredientCategoryRepository,
            IIngredientRepository ingredientRepository)
        {
            _userDietRestrictionRepository = userDietRestrictionRepository;
            _mapper = mapper;
            _ingredientCategoryRepository = ingredientCategoryRepository;
            _ingredientRepository = ingredientRepository;
        }

        public async Task CreateIngredientCategoryRestrictionAsync(Guid userId, CreateIngredientCategoryRestrictionRequest request)
        {
            if (request.ExpiredAtUtc != null && request.ExpiredAtUtc < DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var exist = await _ingredientCategoryRepository.ExistsAsync(i => i.Id == request.IngredientCategoryId);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var type = RestrictionType.From(request.Type);

            var duplicates = await _userDietRestrictionRepository.ExistsAsync(
                u => u.IngredientCategoryId == request.IngredientCategoryId &&
                (u.ExpiredAtUtc == null || u.ExpiredAtUtc > DateTime.UtcNow) &&
                u.UserId == userId);

            if (duplicates)
                throw new AppException(AppResponseCode.DUPLICATE);

            if (type == RestrictionType.TemporaryAvoid && !request.ExpiredAtUtc.HasValue)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (type != RestrictionType.TemporaryAvoid)
            {
                request.ExpiredAtUtc = null;
            }

            await _userDietRestrictionRepository.AddAsync(new UserDietRestriction
            {
                UserId = userId,
                ExpiredAtUtc = request.ExpiredAtUtc,
                IngredientCategoryId = request.IngredientCategoryId,
                Notes = request.Notes,
                Type = type,
            });
        }

        public async Task CreateIngredientRestrictionAsync(Guid userId, CreateIngredientRestrictionRequest request)
        {
            if (request.ExpiredAtUtc != null && request.ExpiredAtUtc < DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var exist = await _ingredientRepository.ExistsAsync(i => i.Id == request.IngredientId);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var type = RestrictionType.From(request.Type);

            var duplicates = await _userDietRestrictionRepository.ExistsAsync(
                u => u.IngredientId == request.IngredientId &&
                (u.ExpiredAtUtc == null || u.ExpiredAtUtc > DateTime.UtcNow) &&
                u.UserId == userId);

            if (duplicates)
                throw new AppException(AppResponseCode.DUPLICATE);

            if (type == RestrictionType.TemporaryAvoid && !request.ExpiredAtUtc.HasValue)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (type != RestrictionType.TemporaryAvoid)
            {
                request.ExpiredAtUtc = null;
            }

            await _userDietRestrictionRepository.AddAsync(new UserDietRestriction
            {
                UserId = userId,
                ExpiredAtUtc = request.ExpiredAtUtc,
                IngredientId = request.IngredientId,
                Notes = request.Notes,
                Type = type,
            });
        }

        public async Task DeleteRestrictionAsync(Guid userId, Guid restrictionId)
        {
            var restriction = await _userDietRestrictionRepository.GetByIdAsync(restrictionId);
            if (restriction == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (restriction.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            await _userDietRestrictionRepository.DeleteAsync(restriction);
        }

        public async Task<IEnumerable<UserDietRestrictionResponse>> GetUserDietRestrictionsAsync(Guid userId, UserDietRestrictionFilterRequest request)
        {
            var restrictions = await _userDietRestrictionRepository.GetAllAsync(
                    predicate: d => d.UserId == userId &&
                                    (d.ExpiredAtUtc == null || d.ExpiredAtUtc > DateTime.UtcNow),
                    include: q => q.Include(d => d.Ingredient)
                                   .Include(d => d.IngredientCategory)
                );

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string keyword = request.Keyword.NormalizeVi();
                restrictions = restrictions
                    .Where(d =>
                        (d.Ingredient != null && d.Ingredient.Name.NormalizeVi().Contains(keyword)) ||
                        (d.IngredientCategory != null && d.IngredientCategory.Name.NormalizeVi().Contains(keyword)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                var type = RestrictionType.From(request.Type);
                restrictions = restrictions.Where(d => d.Type == type).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                restrictions = request.SortBy.ToLower() switch
                {
                    "name_asc" => restrictions
                        .OrderBy(d => d.Ingredient != null ? d.Ingredient.Name : d.IngredientCategory?.Name ?? string.Empty)
                        .ToList(),

                    "name_desc" => restrictions
                        .OrderByDescending(d => d.Ingredient != null ? d.Ingredient.Name : d.IngredientCategory?.Name ?? string.Empty)
                        .ToList(),

                    "type_asc" => restrictions
                        .OrderBy(d => d.Type.Value)
                        .ToList(),

                    "type_desc" => restrictions
                        .OrderByDescending(d => d.Type.Value)
                        .ToList(),

                    "expired_asc" => restrictions
                        .OrderBy(d => d.ExpiredAtUtc == null ? DateTime.MaxValue : d.ExpiredAtUtc)
                        .ToList(),

                    "expired_desc" => restrictions
                        .OrderByDescending(d => d.ExpiredAtUtc == null ? DateTime.MaxValue : d.ExpiredAtUtc)
                        .ToList(),

                    _ => restrictions
                };
            }
            else
            {
                restrictions = restrictions
                    .OrderBy(d => d.Ingredient != null ? d.Ingredient.Name : d.IngredientCategory?.Name ?? string.Empty)
                    .ToList();
            }

            var result = _mapper.Map<IEnumerable<UserDietRestrictionResponse>>(restrictions);

            return result;
        }
    }
}
