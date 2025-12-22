using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations
{
    public class UserHealthGoalService : IUserHealthGoalService
    {
        private readonly IUserHealthGoalRepository _userHealthGoalRepository;
        private readonly IHealthGoalRepository _healthGoalRepopository;
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UserHealthGoalService(IUserHealthGoalRepository userHealthGoalRepository,
            IHealthGoalRepository healthGoalRepopository,
            IMapper mapper,
            ICacheService cacheService,
            ICustomHealthGoalRepository customHealthGoalRepository)
        {
            _userHealthGoalRepository = userHealthGoalRepository;
            _healthGoalRepopository = healthGoalRepopository;
            _mapper = mapper;
            _cacheService = cacheService;
            _customHealthGoalRepository = customHealthGoalRepository;
        }

        public async Task SetGoalAsync(Guid userId, Guid targetId, UserHealthGoalRequest request)
        {
            if (request.ExpiredAtUtc != null && request.ExpiredAtUtc <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thời gian mục tiêu không hợp lệ");

            var now = DateTime.UtcNow;

            var current = await _userHealthGoalRepository.GetActiveGoalByUserIdAsync(userId);

            if (current != null)
            {
                current.ExpiredAtUtc = DateTime.UtcNow;
                await _userHealthGoalRepository.UpdateAsync(current);
            }

            var type = HealthGoalType.From(request.Type);

            if (type == HealthGoalType.SYSTEM)
            {
                var exist = await _healthGoalRepopository.ExistsAsync(u => u.Id == targetId);
                if (!exist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

                var newGoal = new UserHealthGoal
                {
                    UserId = userId,
                    HealthGoalId = targetId,
                    Type = type,
                    StartedAtUtc = DateTime.UtcNow,
                    ExpiredAtUtc = request.ExpiredAtUtc
                };

                await _userHealthGoalRepository.AddAsync(newGoal);
            }
            else if (type == HealthGoalType.CUSTOM)
            {
                var exist = await _customHealthGoalRepository.ExistsAsync(u => u.Id == targetId && u.UserId == userId);
                if (!exist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

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

            await _cacheService.RemoveByPrefixAsync($"recommend:user:{userId}");
        }

        public async Task<UserHealthGoalResponse> GetCurrentGoalAsync(Guid userId)
        {
            var current = await _userHealthGoalRepository.GetActiveGoalByUserIdAsync(userId);

            var result = _mapper.Map<UserHealthGoalResponse>(current);
            return result;
        }

        public async Task RemoveGoalFromCurrentAsync(Guid userId)
        {
            var current = await _userHealthGoalRepository.GetActiveGoalByUserIdAsync(userId);
            if (current == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bạn hiện đang không có mục tiêu sữc khỏe");

            current.ExpiredAtUtc = DateTime.UtcNow;
            await _userHealthGoalRepository.UpdateAsync(current);
            await _cacheService.RemoveByPrefixAsync($"recommend:user:{userId}");
        }

        public async Task<IEnumerable<UserHealthGoalResponse>> GetHistoryGoalAsync(Guid userId)
        {
            var history = await _userHealthGoalRepository.GetHistoryByUserIdAsync(userId);

            var result = _mapper.Map<IEnumerable<UserHealthGoalResponse>>(history);
            return result;
        }
    }
}
