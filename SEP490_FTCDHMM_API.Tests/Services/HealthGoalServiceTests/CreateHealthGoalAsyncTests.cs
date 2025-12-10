using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public class CreateHealthGoalAsyncTests : HealthGoalServiceTestBase
    {
        [Fact]
        public async Task Create_ShouldThrow_WhenNameExists()
        {
            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.HealthGoal, bool>>>()))
                .ReturnsAsync(true);

            var req = new CreateHealthGoalRequest
            {
                Name = "Test",
                Targets = new List<NutrientTargetRequest>()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateHealthGoalAsync(req));
            HealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenDuplicateNutrient()
        {
            var id = NewId();
            var req = new CreateHealthGoalRequest
            {
                Name = "Goal",
                Targets = new List<NutrientTargetRequest>
                {
                    new() { NutrientId = id, TargetType = NutrientTargetType.Absolute.Value, MinValue = 1, MaxValue = 5 },
                    new() { NutrientId = id, TargetType = NutrientTargetType.Absolute.Value, MinValue = 2, MaxValue = 6 }
                }
            };

            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<HealthGoal, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateHealthGoalAsync(req));
        }

        [Fact]
        public async Task Create_ShouldSucceed()
        {
            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<HealthGoal, bool>>>()))
                .ReturnsAsync(false);

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            HealthGoalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<HealthGoal>()))
                .ReturnsAsync(new HealthGoal());

            var req = new CreateHealthGoalRequest
            {
                Name = "Goal",
                Targets = new List<NutrientTargetRequest>
                {
                    new()
                    {
                        NutrientId = NewId(),
                        TargetType = NutrientTargetType.Absolute.Value,
                        MinValue = 1,
                        MaxValue = 5
                    }
                }
            };

            await Sut.CreateHealthGoalAsync(req);
            HealthGoalRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
        }
    }
}
