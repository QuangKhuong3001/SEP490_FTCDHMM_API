using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CustomHealthGoalServiceTests
{
    public class CreateCustomHealthGoalAsyncTests : CustomHealthGoalServiceTestBase
    {
        [Fact]
        public async Task CreateCustomHealthGoalAsync_ShouldThrow_WhenDuplicateNutrient()
        {
            var req = new CreateCustomHealthGoalRequest
            {
                Targets =
                {
                    new NutrientTargetRequest { NutrientId = Guid.Parse("00000000-0000-0000-0000-000000000001"), TargetType="EnergyPercent", MaxEnergyPct = 20 },
                    new NutrientTargetRequest { NutrientId = Guid.Parse("00000000-0000-0000-0000-000000000001"), TargetType="EnergyPercent", MaxEnergyPct = 10 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCustomHealthGoalAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateCustomHealthGoalAsync_ShouldThrow_WhenTotalPctExceeds100()
        {
            var req = new CreateCustomHealthGoalRequest
            {
                Targets =
                {
                    new NutrientTargetRequest { NutrientId = Guid.NewGuid(), TargetType="EnergyPercent", MaxEnergyPct = 60 },
                    new NutrientTargetRequest { NutrientId = Guid.NewGuid(), TargetType="EnergyPercent", MaxEnergyPct = 50 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCustomHealthGoalAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateCustomHealthGoalAsync_ShouldThrow_WhenNutrientNotFound()
        {
            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new CreateCustomHealthGoalRequest
            {
                Targets =
                {
                    new NutrientTargetRequest { NutrientId = Guid.NewGuid(), TargetType="EnergyPercent", MaxEnergyPct = 20 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCustomHealthGoalAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateCustomHealthGoalAsync_ShouldCreateSuccessfully()
        {
            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            CustomHealthGoalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<CustomHealthGoal>()))
                .ReturnsAsync(new CustomHealthGoal())
                .Verifiable();

            var req = new CreateCustomHealthGoalRequest
            {
                Name = "Test Goal",
                Targets =
                {
                    new NutrientTargetRequest
                    {
                        NutrientId = Guid.NewGuid(),
                        TargetType = "Absolute",
                        MinValue = 10,
                        MaxValue = 20,
                        Weight = 1
                    }
                }
            };

            await Sut.CreateCustomHealthGoalAsync(Guid.NewGuid(), req);

            CustomHealthGoalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CustomHealthGoal>()), Times.Once);
        }
    }
}
