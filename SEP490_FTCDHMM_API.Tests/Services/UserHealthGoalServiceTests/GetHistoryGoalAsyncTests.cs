using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthGoalServiceTests
{
    public class GetHistoryGoalAsyncTests : UserHealthGoalServiceTestBase
    {
        [Fact]
        public async Task GetHistory_ShouldReturnMappedList()
        {
            var userId = NewId();

            var list = new List<UserHealthGoal>
            {
                new UserHealthGoal { UserId = userId, HealthGoalId = NewId() }
            };

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetHistoryByUserIdAsync(userId))
                .ReturnsAsync(list);

            MapperMock
                .Setup(m => m.Map<IEnumerable<UserHealthGoalResponse>>(list))
                .Returns(new List<UserHealthGoalResponse>
                {
                    new UserHealthGoalResponse { HealthGoalId = list[0].HealthGoalId }
                });

            var result = await Sut.GetHistoryGoalAsync(userId);

            Assert.Single(result);

            UserHealthGoalRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
