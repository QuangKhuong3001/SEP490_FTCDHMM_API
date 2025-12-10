using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class CreateReportAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task CreateReportAsync_ShouldThrow_WhenReporterNotFound()
        {
            var reporterId = Guid.NewGuid();
            var request = new ReportRequest
            {
                TargetId = Guid.NewGuid(),
                TargetType = "USER",
                Description = "abc"
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(reporterId, null))
                .ReturnsAsync((AppUser)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateReportAsync(reporterId, request));
        }

        [Fact]
        public async Task CreateReportAsync_ShouldUpdateExistingReport_WhenPendingExists()
        {
            var reporterId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var existing = CreateReport(reporterId, targetId, "old");

            var request = new ReportRequest
            {
                TargetId = targetId,
                TargetType = "USER",
                Description = "new text"
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(reporterId, null))
                .ReturnsAsync(CreateUser(reporterId));

            ReportRepoMock
                 .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Expression<Func<Report, bool>>?>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>?>()
                ))
                .ReturnsAsync(existing);

            ReportRepoMock
                .Setup(r => r.UpdateAsync(existing))
                .Returns(Task.CompletedTask);

            await Sut.CreateReportAsync(reporterId, request);

            Assert.Equal("new text", existing.Description);

            ReportRepoMock.Verify(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Report, bool>>>(),
                It.IsAny<Expression<Func<Report, bool>>?>(),
                It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>?>()), Times.Once);

            ReportRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task CreateReportAsync_ShouldAddNewReport()
        {
            var reporterId = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            var request = new ReportRequest
            {
                TargetId = targetId,
                TargetType = "USER",
                Description = "abc"
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(reporterId, null))
                .ReturnsAsync(CreateUser(reporterId));

            ReportRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Expression<Func<Report, bool>>?>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>?>()
                ))
                .ReturnsAsync((Report)null!);

            UserRepoMock
                .Setup(r => r.ExistsAsync(u => u.Id == targetId))
                .ReturnsAsync(true);

            Report? added = null;

            ReportRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Report>()))
                .Callback<Report>(r => added = r)
                .ReturnsAsync(() => added!);

            await Sut.CreateReportAsync(reporterId, request);

            Assert.NotNull(added);
            Assert.Equal(reporterId, added!.ReporterId);
            Assert.Equal(targetId, added.TargetId);
            Assert.Equal("abc", added.Description);
            Assert.Equal(ReportStatus.Pending, added.Status);
            Assert.Equal(ReportObjectType.User, added.TargetType);

            ReportRepoMock.Verify(r => r.AddAsync(It.IsAny<Report>()), Times.Once);
        }

        [Fact]
        public async Task CreateReportAsync_ShouldThrow_WhenReportYourself()
        {
            var reporterId = Guid.NewGuid();

            var request = new ReportRequest
            {
                TargetId = reporterId,
                TargetType = "USER",
                Description = "abc"
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(reporterId, null))
                .ReturnsAsync(CreateUser(reporterId));

            ReportRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Expression<Func<Report, bool>>?>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>?>()
                ))
                .ReturnsAsync((Report)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateReportAsync(reporterId, request));

            UserRepoMock.Verify(r => r.GetByIdAsync(reporterId, null), Times.Once);
        }

        [Fact]
        public async Task CreateReportAsync_ShouldReturn_WhenExistingReportAndEmptyDescription()
        {
            var reporterId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var existing = CreateReport(reporterId, targetId, "old");

            var request = new ReportRequest
            {
                TargetId = targetId,
                TargetType = "USER",
                Description = "   "
            };

            UserRepoMock
                .Setup(r => r.GetByIdAsync(reporterId, null))
                .ReturnsAsync(CreateUser(reporterId));

            ReportRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Report, bool>>>(),
                    It.IsAny<Expression<Func<Report, bool>>?>(),
                    It.IsAny<Func<IQueryable<Report>, IQueryable<Report>>?>()
                ))
                .ReturnsAsync(existing);

            await Sut.CreateReportAsync(reporterId, request);

            ReportRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Report>()), Times.Never);
            ReportRepoMock.Verify(r => r.AddAsync(It.IsAny<Report>()), Times.Never);
        }
    }
}
