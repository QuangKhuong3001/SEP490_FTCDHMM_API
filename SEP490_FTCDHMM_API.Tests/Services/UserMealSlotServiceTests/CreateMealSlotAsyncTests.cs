using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserMealSlotServiceTests
{
    public class CreateMealSlotAsyncTests : UserMealSlotServiceTestBase
    {
        [Fact]
        public async Task Create_ShouldThrow_WhenEnergyPercentInvalid()
        {
            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateMealSlotAsync(NewId(), new MealSlotRequest
                {
                    Name = "Breakfast",
                    EnergyPercent = 0,
                    OrderIndex = 1
                }));
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenOrderIndexExists()
        {
            var userId = NewId();

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(new List<UserMealSlot>
                {
                    new UserMealSlot { OrderIndex = 1, EnergyPercent = 0.3m }
                });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateMealSlotAsync(userId, new MealSlotRequest
                {
                    Name = "Lunch",
                    EnergyPercent = 0.3m,
                    OrderIndex = 1
                }));

            MealSlotRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenEnergySumExceedsOne()
        {
            var userId = NewId();

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(new List<UserMealSlot>
                {
                    new UserMealSlot { EnergyPercent = 0.8m }
                });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateMealSlotAsync(userId, new MealSlotRequest
                {
                    Name = "Dinner",
                    EnergyPercent = 0.3m,
                    OrderIndex = 2
                }));

            MealSlotRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Create_ShouldAddSlot_WhenValid()
        {
            var userId = NewId();

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(new List<UserMealSlot>());

            MealSlotRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserMealSlot>()))
                .ReturnsAsync((UserMealSlot slot) => slot);

            await Sut.CreateMealSlotAsync(userId, new MealSlotRequest
            {
                Name = "Breakfast",
                EnergyPercent = 0.3m,
                OrderIndex = 1
            });

            MealSlotRepositoryMock.VerifyAll();
        }
    }
}
