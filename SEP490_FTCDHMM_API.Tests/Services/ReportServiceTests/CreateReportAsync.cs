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
        private void SetupPendingReport(Guid reporterId, Guid targetId, ReportObjectType type, Report? result)
        {
            ReportRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Report, DateTime>>>(),
                    It.Is<Expression<Func<Report, bool>>>(p =>
                        p.Compile().Invoke(new Report
                        {
                            ReporterId = reporterId,
                            TargetId = targetId,
                            TargetType = type,
                            Status = ReportStatus.Pending
                        })
                    ),
                    null
                ))
                .ReturnsAsync(result);
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

            SetupPendingReport(reporterId, targetId, ReportObjectType.User, existing);

            ReportRepoMock
                .Setup(r => r.UpdateAsync(existing))
                .Returns(Task.CompletedTask);

            await Sut.CreateReportAsync(reporterId, request);

            Assert.Equal("new text", existing.Description);
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

            SetupPendingReport(reporterId, targetId, ReportObjectType.User, null);

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

            SetupPendingReport(reporterId, reporterId, ReportObjectType.User, null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateReportAsync(reporterId, request));
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

            SetupPendingReport(reporterId, targetId, ReportObjectType.User, existing);

            await Sut.CreateReportAsync(reporterId, request);
        }
    }
}
