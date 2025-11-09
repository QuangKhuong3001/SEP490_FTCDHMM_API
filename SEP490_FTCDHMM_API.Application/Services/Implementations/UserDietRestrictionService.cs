using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

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

        public async Task CreateIngredientCategoryRestriction(Guid userId, CreateIngredientCategoryRestrictionRequest request)
        {
            if (request.ExpiredAtUtc < DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var exist = await _ingredientCategoryRepository.ExistsAsync(i => i.Id == request.IngredientCategoryId);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var duplicates = await _userDietRestrictionRepository.ExistsAsync(
                u => u.IngredientCategoryId == request.IngredientCategoryId &&
                u.UserId == userId);

            if (duplicates)
                throw new AppException(AppResponseCode.DUPLICATE);

            var type = RestrictionType.From(request.Type);

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

        public async Task CreateIngredientRestriction(Guid userId, CreateIngredientRestrictionRequest request)
        {
            if (request.ExpiredAtUtc < DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var exist = await _ingredientRepository.ExistsAsync(i => i.Id == request.IngredientId);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var duplicates = await _userDietRestrictionRepository.ExistsAsync(
                u => u.IngredientId == request.IngredientId &&
                u.UserId == userId);

            if (duplicates)
                throw new AppException(AppResponseCode.DUPLICATE);

            var type = RestrictionType.From(request.Type);

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

        public async Task DeleteRestriction(Guid userId, Guid restrictionId)
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
                                    (d.ExpiredAtUtc == null || d.ExpiredAtUtc < DateTime.UtcNow),
                    include: q => q.Include(d => d.Ingredient)
                                   .Include(d => d.IngredientCategory)
                );

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string keyword = request.Keyword.Trim().ToLower();
                restrictions = restrictions
                    .Where(d =>
                        (d.Ingredient != null && d.Ingredient.Name.ToLower().Contains(keyword)) ||
                        (d.IngredientCategory != null && d.IngredientCategory.Name.ToLower().Contains(keyword)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                var type = RestrictionType.From(request.Type);
                restrictions = restrictions.Where(d => d.Type == type).ToList();
            }

            restrictions = restrictions
                .OrderBy(d => d.Ingredient != null ? d.Ingredient.Name : d.IngredientCategory!.Name)
                .ToList();

            var result = _mapper.Map<IEnumerable<UserDietRestrictionResponse>>(restrictions);

            return result;
        }
    }
}
