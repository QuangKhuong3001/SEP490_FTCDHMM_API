using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthMetricServiceTests
{
    public class CreateHealthMetricAsyncTests : UserHealthMetricServiceTestBase
    {

        [Fact]
        public async Task CreateHealthMetricAsync_ShouldCreateMetric()
        {
            var userId = Guid.NewGuid();
            var user = new AppUser
            {
                Id = userId,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.VeryActive
            };

            var request = new CreateUserHealthMetricRequest
            {
                HeightCm = 180,
                WeightKg = 80,
                BodyFatPercent = 15,
                MuscleMassKg = 30,
                Notes = "Test"
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            MetricRepoMock
                .Setup(r => r.AddAsync(It.IsAny<UserHealthMetric>()))
                .ReturnsAsync((UserHealthMetric uh) => uh)
                .Verifiable();

            await Sut.CreateHealthMetricAsync(userId, request);

            MetricRepoMock.Verify(r => r.AddAsync(It.IsAny<UserHealthMetric>()), Times.Once);
        }

    }
}
