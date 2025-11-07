using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserHealthGoalService : IUserHealthGoalService
    {
        private readonly IUserHealthGoalRepository _userHealthGoalRepository;
        private readonly IHealthGoalRepository _healthGoalRepopository;
        private readonly IHealthGoalConflictRepository _healthGoalConflictRepository;
        private readonly IMapper _mapper;

        public UserHealthGoalService(IUserHealthGoalRepository userHealthGoalRepository,
            IHealthGoalRepository healthGoalRepopository,
            IMapper mapper,
            IHealthGoalConflictRepository healthGoalConflictRepository)
        {
            _userHealthGoalRepository = userHealthGoalRepository;
            _healthGoalRepopository = healthGoalRepopository;
            _mapper = mapper;
            _healthGoalConflictRepository = healthGoalConflictRepository;
        }

        public async Task SetGoalAsync(Guid userId, Guid healthGoalId)
        {
            var goal = await _healthGoalRepopository.GetByIdAsync(healthGoalId);
            if (goal == null) throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userHealthGoalRepository.ExistsAsync(c => c.UserId == userId && c.HealthGoalId == healthGoalId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var conflicts = await _healthGoalConflictRepository.GetAllAsync(
                c => c.HealthGoalAId == healthGoalId || c.HealthGoalBId == healthGoalId
            );

            var conflictingGoalIds = conflicts
                .Select(c => c.HealthGoalAId == healthGoalId ? c.HealthGoalBId : c.HealthGoalAId)
                .Distinct()
                .ToList();

            var userConflictGoals = await _userHealthGoalRepository.GetAllAsync(
                ug => ug.UserId == userId && conflictingGoalIds.Contains(ug.HealthGoalId)
            );

            await _userHealthGoalRepository.DeleteRangeAsync(userConflictGoals);

            var userGoal = new UserHealthGoal { UserId = userId, HealthGoalId = healthGoalId };
            await _userHealthGoalRepository.AddAsync(userGoal);
        }

        public async Task<IEnumerable<HealthGoalResponse>> GetCurrentGoalAsync(Guid userId)
        {
            var userGoals = await _userHealthGoalRepository.GetAllAsync(u => u.UserId == userId, include: q => q.Include(u => u.HealthGoal).ThenInclude(g => g.Targets));

            var currentHealthGoalIds = userGoals.ToList().Select(c => c.HealthGoalId).ToList();
            var currentHealthGoal = await _healthGoalRepopository.GetAllAsync(
                h => currentHealthGoalIds.Contains(h.Id),
                include: q => q.Include(h => h.Targets).ThenInclude(t => t.Nutrient));

            var result = _mapper.Map<IEnumerable<HealthGoalResponse>>(currentHealthGoal);
            return result;
        }

        public async Task RemoveFromCurrent(Guid userId, Guid healthGoalId)
        {
            var goal = await _healthGoalRepopository.GetByIdAsync(healthGoalId);
            if (goal == null) throw new AppException(AppResponseCode.NOT_FOUND);

            var healthGoals = await _userHealthGoalRepository.GetAllAsync(c => c.UserId == userId && c.HealthGoalId == healthGoalId);

            var userHealthGoal = healthGoals.FirstOrDefault();

            if (userHealthGoal == null)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userHealthGoalRepository.DeleteAsync(userHealthGoal);
        }
    }
}
