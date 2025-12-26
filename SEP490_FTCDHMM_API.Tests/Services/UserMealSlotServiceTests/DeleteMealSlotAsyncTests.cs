using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserMealSlotServiceTests
{
    public class DeleteMealSlotAsyncTests : UserMealSlotServiceTestBase
    {
        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((UserMealSlot)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteMealSlotAsync(NewId(), NewId()));

            MealSlotRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenUserMismatch()
        {
            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(new UserMealSlot
                {
                    UserId = NewId()
                });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteMealSlotAsync(NewId(), NewId()));

            MealSlotRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldRemove_WhenValid()
        {
            var userId = NewId();
            var slot = new UserMealSlot { UserId = userId };

            MealSlotRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(slot);

            MealSlotRepositoryMock
                .Setup(r => r.DeleteAsync(slot))
                .Returns(Task.CompletedTask);

            await Sut.DeleteMealSlotAsync(userId, NewId());

            MealSlotRepositoryMock.VerifyAll();
        }
    }
}
