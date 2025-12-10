using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class GetReportDetailsAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task GetReportDetailsAsync_ShouldThrow_WhenNoReports()
        {
            var targetId = Guid.NewGuid();

            ReportRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>>()))
                .ReturnsAsync(new List<Report>());

            await Assert.ThrowsAsync<AppException>(() => Sut.GetReportDetailsAsync(targetId, "USER"));
        }

        [Fact]
        public async Task GetReportDetailsAsync_ShouldReturnDetails()
        {
            var targetId = Guid.NewGuid();
            var reporter = CreateUser(Guid.NewGuid());

            var reports = new List<Report>
            {
                new Report
                {
                    Id = Guid.NewGuid(),
                    TargetId = targetId,
                    TargetType = ReportObjectType.User,
                    Description = "abc",
                    Reporter = reporter,
                    Status = ReportStatus.Pending,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            ReportRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>>()))
                .ReturnsAsync(reports);

            UserRepoMock.Setup(r => r.GetByIdAsync(targetId, null)).ReturnsAsync(CreateUser(targetId));

            var result = await Sut.GetReportDetailsAsync(targetId, "USER");

            Assert.Equal(targetId, result.TargetId);
            Assert.Single(result.Reports);
        }
    }
}
