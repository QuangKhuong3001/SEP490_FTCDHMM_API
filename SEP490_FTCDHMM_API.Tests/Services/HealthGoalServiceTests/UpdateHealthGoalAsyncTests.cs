using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public class UpdateHealthGoalAsyncTests : HealthGoalServiceTestBase
    {
        [Fact]
        public async Task Update_ShouldThrow_WhenNotFound()
        {
            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync((HealthGoal)null!);

            var req = new UpdateHealthGoalRequest
            {
                Description = "X",
                Targets = new List<NutrientTargetRequest>()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateHealthGoalAsync(NewId(), req));
            HealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Update_ShouldSucceed()
        {
            var now = DateTime.UtcNow;

            var old = new HealthGoal
            {
                Id = NewId(),
                Description = "Old",
                LastUpdatedUtc = now,
                Targets = new List<HealthGoalTarget>
                {
                    new()
                    {
                        Id = NewId(),
                        NutrientId = NewId()
                    }
                }
            };

            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    old.Id,
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync(old);

            NutrientRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(new Nutrient
                {
                    IsMacroNutrient = false
                });

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            HealthGoalTargetRepositoryMock
                .Setup(r => r.DeleteRangeAsync(old.Targets))
                .Returns(Task.CompletedTask);

            HealthGoalRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<HealthGoal>()))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(c => c.RemoveByPrefixAsync("health-goal"))
                .Returns(Task.CompletedTask);

            var req = new UpdateHealthGoalRequest
            {
                Description = "New",
                LastUpdatedUtc = now,
                Targets = new List<NutrientTargetRequest>
                {
                    new()
                    {
                        NutrientId = NewId(),
                        TargetType = NutrientTargetType.Absolute.Value,
                        MinValue = 1,
                        MaxValue = 5,
                        Weight = 1
                    }
                }
            };

            await Sut.UpdateHealthGoalAsync(old.Id, req);

            HealthGoalRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
            HealthGoalTargetRepositoryMock.VerifyAll();
            CacheServiceMock.VerifyAll();
        }


    }
}
