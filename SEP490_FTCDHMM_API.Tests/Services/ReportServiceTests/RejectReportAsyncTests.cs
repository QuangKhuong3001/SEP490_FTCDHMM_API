using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public class RejectReportAsyncTests : ReportServiceTestBase
    {
        [Fact]
        public async Task RejectReportAsync_ShouldThrow_WhenReasonEmpty()
        {
            await Assert.ThrowsAsync<AppException>(() =>
                Sut.RejectReportAsync(Guid.NewGuid(), Guid.NewGuid(), ""));
        }

        [Fact]
        public async Task RejectReportAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();

            ReportRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync((Report)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.RejectReportAsync(id, Guid.NewGuid(), "x"));
        }

        [Fact]
        public async Task RejectReportAsync_ShouldThrow_WhenAlreadyRejected()
        {
            var report = new Report { Status = ReportStatus.Rejected };

            ReportRepoMock.Setup(r => r.GetByIdAsync(report.Id, null)).ReturnsAsync(report);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.RejectReportAsync(report.Id, Guid.NewGuid(), "x"));
        }

        [Fact]
        public async Task RejectReportAsync_ShouldUpdateStatus()
        {
            var report = new Report { Status = ReportStatus.Pending };

            ReportRepoMock.Setup(r => r.GetByIdAsync(report.Id, null)).ReturnsAsync(report);
            ReportRepoMock.Setup(r => r.UpdateAsync(report)).Returns(Task.CompletedTask);

            await Sut.RejectReportAsync(report.Id, Guid.NewGuid(), "reason");

            Assert.Equal(ReportStatus.Rejected, report.Status);
            ReportRepoMock.Verify(r => r.UpdateAsync(report), Times.Once);
        }
    }
}
