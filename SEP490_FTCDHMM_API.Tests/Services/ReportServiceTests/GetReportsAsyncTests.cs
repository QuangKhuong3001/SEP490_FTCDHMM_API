using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class GetReportsAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task GetReportsAsync_ShouldReturnPagedGroupedResults()
        {
            var request = new ReportFilterRequest
            {
                Type = "USER",
                PaginationParams = new SEP490_FTCDHMM_API.Application.Dtos.Common.PaginationParams
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            var reports = new List<Report>
            {
                new Report
                {
                    TargetType = ReportObjectType.User,
                    TargetId = Guid.NewGuid(),
                    Status = ReportStatus.Pending,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Report
                {
                    TargetType = ReportObjectType.User,
                    TargetId = Guid.NewGuid(),
                    Status = ReportStatus.Pending,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            ReportRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>>()))
                .ReturnsAsync(reports);

            UserRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync(CreateUser(Guid.NewGuid()));

            var result = await Sut.GetReportsAsync(request);

            Assert.True(result.Items.Any());
            Assert.Equal(2, result.TotalCount);
        }
    }
}
