using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class UpdateColorCodeAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task UpdateColor_ShouldThrowNotFound_WhenLabelMissing()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateColorCodeRequest
            {
                ColorCode = "#111",
                LastUpdatedUtc = DateTime.UtcNow
            };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, null))
                .ReturnsAsync((Label?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateColorCodeAsync(id, dto));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateColor_ShouldThrowConflict_WhenLastUpdatedMismatch()
        {
            var id = Guid.NewGuid();
            var label = CreateLabel(id);
            label.LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-5);

            var dto = new UpdateColorCodeRequest
            {
                ColorCode = "#111",
                LastUpdatedUtc = DateTime.UtcNow
            };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, null))
                .ReturnsAsync(label);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateColorCodeAsync(id, dto));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateColor_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var lastUpdated = DateTime.UtcNow;

            var label = CreateLabel(id);
            label.LastUpdatedUtc = lastUpdated;

            var dto = new UpdateColorCodeRequest
            {
                ColorCode = "#222",
                LastUpdatedUtc = lastUpdated
            };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, null))
                .ReturnsAsync(label);

            LabelRepositoryMock
                .Setup(r => r.UpdateAsync(label))
                .Returns(Task.CompletedTask);

            await Sut.UpdateColorCodeAsync(id, dto);

            Assert.Equal("#222", label.ColorCode);
            Assert.True(label.LastUpdatedUtc > lastUpdated);

            LabelRepositoryMock.VerifyAll();
        }
    }
}
