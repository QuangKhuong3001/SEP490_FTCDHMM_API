using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserHealthMetricService : IUserHealthMetricService
    {
        private readonly IUserHealthMetricRepository _userHealthMetricRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserHealthMetricService(IUserHealthMetricRepository userHealthMetricRepository, IMapper mapper, IUserRepository userRepository)
        {
            _userHealthMetricRepository = userHealthMetricRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task CreateHealthMetricAsync(Guid userId, CreateUserHealthMetricRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var bmi = request.WeightKg / (request.HeightCm / 100m * request.HeightCm / 100m);

            var age = AgeCalculator.Calculate(user!.DateOfBirth);

            var bmr = BmrCalculator.Calculate(
                weightKg: request.WeightKg,
                heightCm: request.HeightCm,
                age: age,
                gender: user.Gender.Value,
                muscleMassKg: request.MuscleMassKg,
                bodyFatPercent: request.BodyFatPercent);

            var activityFactor = user.ActivityLevel.Factor;

            var tdee = bmr * activityFactor;

            var metric = new UserHealthMetric
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WeightKg = request.WeightKg,
                HeightCm = request.HeightCm,
                BMI = Math.Round(bmi, 2),
                BodyFatPercent = request.BodyFatPercent,
                MuscleMassKg = request.MuscleMassKg,
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                Notes = request.Notes,
                RecordedAt = DateTime.UtcNow,
                ActivityLevel = user.ActivityLevel
            };

            await _userHealthMetricRepository.AddAsync(metric);
        }

        public async Task UpdateHealthMetricAsync(Guid userId, Guid metricId, UpdateUserHealthMetricRequest request)
        {
            var metric = await _userHealthMetricRepository.GetByIdAsync(metricId);

            if (metric == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var user = await _userRepository.GetByIdAsync(userId);

            if (metric.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            var bmi = request.WeightKg / (request.HeightCm / 100m * request.HeightCm / 100m);

            var age = AgeCalculator.Calculate(user!.DateOfBirth);

            var bmr = BmrCalculator.Calculate(
                weightKg: request.WeightKg,
                heightCm: request.HeightCm,
                age: age,
                gender: user.Gender.Value,
                muscleMassKg: request.MuscleMassKg,
                bodyFatPercent: request.BodyFatPercent);

            var activityFactor = user.ActivityLevel.Factor;

            var tdee = bmr * activityFactor;

            metric.WeightKg = request.WeightKg;
            metric.HeightCm = request.HeightCm;
            metric.BMI = Math.Round(bmi, 2);
            metric.BodyFatPercent = request.BodyFatPercent;
            metric.MuscleMassKg = request.MuscleMassKg;
            metric.BMR = Math.Round(bmr, 2);
            metric.TDEE = Math.Round(tdee, 2);
            metric.Notes = request.Notes;
            metric.RecordedAt = DateTime.UtcNow;
            metric.ActivityLevel = user.ActivityLevel;

            await _userHealthMetricRepository.UpdateAsync(metric);
        }

        public async Task DeleteHealthMetricAsync(Guid userId, Guid metricId)
        {
            var metric = await _userHealthMetricRepository.GetByIdAsync(metricId);

            if (metric == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (metric.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            await _userHealthMetricRepository.DeleteAsync(metric);
        }

        public async Task<IEnumerable<UserHealthMetricResponse>> GetHealthMetricHistoryByUserIdAsync(Guid userId)
        {
            var metrics = await _userHealthMetricRepository.GetAllAsync(h => h.UserId == userId);

            return _mapper.Map<IEnumerable<UserHealthMetricResponse>>(metrics);
        }

    }
}
