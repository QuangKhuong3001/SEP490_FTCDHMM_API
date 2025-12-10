using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class ApproveReportAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task ApproveReportAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();

            ReportRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync((Report)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.ApproveReportAsync(id, Guid.NewGuid()));
        }

        [Fact]
        public async Task ApproveReportAsync_ShouldThrow_WhenAlreadyApproved()
        {
            var report = new Report
            {
                Status = ReportStatus.Approved
            };

            ReportRepoMock.Setup(r => r.GetByIdAsync(report.Id, null)).ReturnsAsync(report);

            await Assert.ThrowsAsync<AppException>(() => Sut.ApproveReportAsync(report.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task ApproveReportAsync_ShouldUpdateStatus()
        {
            var report = new Report
            {
                Status = ReportStatus.Pending
            };

            ReportRepoMock.Setup(r => r.GetByIdAsync(report.Id, null)).ReturnsAsync(report);
            ReportRepoMock.Setup(r => r.UpdateAsync(report)).Returns(Task.CompletedTask);

            await Sut.ApproveReportAsync(report.Id, Guid.NewGuid());

            Assert.Equal(ReportStatus.Approved, report.Status);
            ReportRepoMock.Verify(r => r.UpdateAsync(report), Times.Once);
        }
    }
}
