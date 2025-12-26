using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.UserMealSlotServiceTests
{
    public class GetMyMealsAsyncTests : UserMealSlotServiceTestBase
    {
        [Fact]
        public async Task GetMyMeals_ShouldReturnOrderedMappedList()
        {
            var userId = NewId();
            var slots = new List<UserMealSlot>
            {
                new UserMealSlot { OrderIndex = 2 },
                new UserMealSlot { OrderIndex = 1 }
            };

            MealSlotRepositoryMock
                .Setup(r => r.GetByUserAsync(userId))
                .ReturnsAsync(slots);

            MapperMock
                .Setup(m => m.Map<List<MealSlotResponse>>(It.IsAny<IEnumerable<UserMealSlot>>()))
                .Returns(new List<MealSlotResponse>
                {
                    new MealSlotResponse { OrderIndex = 1 },
                    new MealSlotResponse { OrderIndex = 2 }
                });

            var result = await Sut.GetMyMealsAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].OrderIndex);

            MealSlotRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
