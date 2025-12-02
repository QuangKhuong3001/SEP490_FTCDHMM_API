using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImp
{
    public class UserHealthGoalService : IUserHealthGoalService
    {
        private readonly IUserHealthGoalRepository _userHealthGoalRepository;
        private readonly IHealthGoalRepository _healthGoalRepopository;
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;
        private readonly IMapper _mapper;

        public UserHealthGoalService(IUserHealthGoalRepository userHealthGoalRepository,
            IHealthGoalRepository healthGoalRepopository,
            IMapper mapper,
            ICustomHealthGoalRepository customHealthGoalRepository)
        {
            _userHealthGoalRepository = userHealthGoalRepository;
            _healthGoalRepopository = healthGoalRepopository;
            _mapper = mapper;
            _customHealthGoalRepository = customHealthGoalRepository;
        }

        public async Task SetGoalAsync(Guid userId, Guid targetId, UserHealthGoalRequest request)
        {
            if (request.ExpiredAtUtc != null && request.ExpiredAtUtc <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thời gian mục tiêu không hợp lệ");

            var type = HealthGoalType.From(request.Type);

            if (type == HealthGoalType.SYSTEM)
            {
                var goalExist = await _healthGoalRepopository.ExistsAsync(u => u.Id == targetId);
                if (!goalExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

                // Expire ALL active goals for this user
                var now = DateTime.UtcNow;
                var activeGoals = await _userHealthGoalRepository.GetAllAsync(
                    c => c.UserId == userId &&
                    c.StartedAtUtc <= now &&
                    (c.ExpiredAtUtc == null || c.ExpiredAtUtc > now)
                );

                foreach (var activeGoal in activeGoals)
                {
                    activeGoal.ExpiredAtUtc = DateTime.UtcNow;
                    await _userHealthGoalRepository.UpdateAsync(activeGoal);
                }

                var newCustomGoal = new UserHealthGoal
                {
                    UserId = userId,
                    HealthGoalId = targetId,
                    Type = type,
                    StartedAtUtc = DateTime.UtcNow,
                    ExpiredAtUtc = request.ExpiredAtUtc
                };

                await _userHealthGoalRepository.AddAsync(newCustomGoal);
            }
            else if (type == HealthGoalType.CUSTOM)
            {
                var customExist = await _customHealthGoalRepository.ExistsAsync(u => u.Id == targetId);
                if (!customExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

                // Expire ALL active goals for this user
                var now = DateTime.UtcNow;
                var activeGoals = await _userHealthGoalRepository.GetAllAsync(
                    c => c.UserId == userId &&
                    c.StartedAtUtc <= now &&
                    (c.ExpiredAtUtc == null || c.ExpiredAtUtc > now)
                );

                foreach (var activeGoal in activeGoals)
                {
                    activeGoal.ExpiredAtUtc = DateTime.UtcNow;
                    await _userHealthGoalRepository.UpdateAsync(activeGoal);
                }

                var newCustomGoal = new UserHealthGoal
                {
                    UserId = userId,
                    CustomHealthGoalId = targetId,
                    Type = type,
                    StartedAtUtc = DateTime.UtcNow,
                    ExpiredAtUtc = request.ExpiredAtUtc
                };

                await _userHealthGoalRepository.AddAsync(newCustomGoal);
            }
        }

        public async Task<UserHealthGoalResponse> GetCurrentGoalAsync(Guid userId)
        {
            var current = await _userHealthGoalRepository.GetActiveGoalByUserIdAsync(userId);

            var result = _mapper.Map<UserHealthGoalResponse>(current);
            return result;
        }

        public async Task RemoveFromCurrent(Guid userId)
        {
            var current = await _userHealthGoalRepository.GetActiveGoalByUserIdAsync(userId);
            if (current == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bạn hiện đang không có mục tiêu sữc khỏe");

            current.ExpiredAtUtc = DateTime.UtcNow;
            await _userHealthGoalRepository.UpdateAsync(current);
        }

        public async Task<IEnumerable<UserHealthGoalResponse>> GetHistoryGoalAsync(Guid userId)
        {
            var history = await _userHealthGoalRepository.GetHistoryByUserIdAsync(userId);

            var result = _mapper.Map<IEnumerable<UserHealthGoalResponse>>(history);
            return result;
        }
    }
}
