using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
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

            var goalExist = await _healthGoalRepopository.ExistsAsync(u => u.Id == targetId);
            var customExist = await _healthGoalRepopository.ExistsAsync(u => u.Id == targetId);
            if (!goalExist && !customExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

            var exist = await _userHealthGoalRepository.GetLatestAsync(c => c.UserId == userId);
            if (exist != null)
            {
                await _userHealthGoalRepository.DeleteAsync(exist);
            }

            if (goalExist)
            {
                var newGoal = new UserHealthGoal
                {
                    UserId = userId,
                    HealthGoalId = targetId,
                    ExpiredAtUtc = request.ExpiredAtUtc
                };
                await _userHealthGoalRepository.AddAsync(newGoal);
            }
            else
            {
                var newCustomGoal = new UserHealthGoal
                {
                    UserId = userId,
                    CustomHealthGoalId = targetId,
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

        public async Task RemoveFromCurrent(Guid userId, Guid Id)
        {
            var userHealthGoals = await _userHealthGoalRepository.GetLatestAsync(c => c.UserId == userId);
            if (userHealthGoals == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bạn hiện đang không có mục tiêu sữc khỏe");

            await _userHealthGoalRepository.DeleteAsync(userHealthGoals);
        }
    }
}
