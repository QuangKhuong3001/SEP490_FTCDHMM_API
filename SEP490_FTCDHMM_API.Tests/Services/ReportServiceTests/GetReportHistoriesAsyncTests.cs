using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class GetReportHistoriesAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task GetReportHistoriesAsync_ShouldReturnHistoryList()
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
                    TargetId = Guid.NewGuid(),
                    TargetType = ReportObjectType.User,
                    Status = ReportStatus.Approved,
                    ReviewedAtUtc = DateTime.UtcNow
                }
            };

            ReportRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>>()))
                .ReturnsAsync(reports);

            UserRepoMock
                .Setup(u => u.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(CreateUser(Guid.NewGuid()));

            var result = await Sut.GetReportHistoriesAsync(request);

            Assert.Single(result.Items);
        }
    }
}
