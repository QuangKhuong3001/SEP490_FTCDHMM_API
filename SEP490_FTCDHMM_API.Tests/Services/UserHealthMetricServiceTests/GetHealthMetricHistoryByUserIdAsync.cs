using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthMetricServiceTests
{
    public class GetHistoryTests : UserHealthMetricServiceTestBase
    {
        [Fact]
        public async Task GetHealthMetricHistoryByUserIdAsync_ShouldReturnHistory()
        {
            var userId = Guid.NewGuid();

            var metrics = new List<UserHealthMetric>
            {
                new UserHealthMetric(),
                new UserHealthMetric()
            };

            MetricRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<UserHealthMetric, bool>>>(), null))
                .ReturnsAsync(metrics);


            MapperMock
                .Setup(m => m.Map<IEnumerable<UserHealthMetricResponse>>(metrics))
                .Returns(new List<UserHealthMetricResponse>
                {
                    new UserHealthMetricResponse(),
                    new UserHealthMetricResponse()
                });

            var result = await Sut.GetHealthMetricHistoryByUserIdAsync(userId);

            Assert.Equal(2, result.Count());
        }
    }
}
