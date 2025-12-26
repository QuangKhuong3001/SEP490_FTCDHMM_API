using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserMealSlotServiceTests
{
    public class UpdateMealSlotAsyncTests : UserMealSlotServiceTestBase
    {
        [Fact]
        public async Task Update_ShouldThrow_WhenNotFound()
        {
            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<UserMealSlot>, IQueryable<UserMealSlot>>>()))
                .ReturnsAsync((UserMealSlot?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateMealSlotAsync(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    new MealSlotRequest
                    {
                        Name = "Breakfast",
                        EnergyPercent = 0.3m,
                        OrderIndex = 1
                    }));

            MealSlotRepositoryMock.VerifyAll();
        }


        [Fact]
        public async Task Update_ShouldThrow_WhenOrderIndexExists()
        {
            var userId = NewId();
            var slotId = NewId();

            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(slotId, null))
                .ReturnsAsync(new UserMealSlot { Id = slotId, UserId = userId });

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(new List<UserMealSlot>
                {
                    new UserMealSlot { OrderIndex = 2 }
                });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateMealSlotAsync(userId, slotId, new MealSlotRequest
                {
                    EnergyPercent = 0.3m,
                    OrderIndex = 2
                }));

            MealSlotRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenValid()
        {
            var userId = NewId();
            var slot = new UserMealSlot
            {
                Id = NewId(),
                UserId = userId,
                EnergyPercent = 0.2m
            };

            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(slot.Id, null))
                .ReturnsAsync(slot);

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(new List<UserMealSlot> { slot });

            MealSlotRepositoryMock
                .Setup(r => r.UpdateAsync(slot))
                .Returns(Task.CompletedTask);

            await Sut.UpdateMealSlotAsync(userId, slot.Id, new MealSlotRequest
            {
                Name = "Updated",
                EnergyPercent = 0.3m,
                OrderIndex = 1
            });

            MealSlotRepositoryMock.VerifyAll();
        }
    }
}
