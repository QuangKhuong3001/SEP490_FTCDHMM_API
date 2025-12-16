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
            var dto = new UpdateColorCodeRequest { ColorCode = "#111" };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>?>()))
                .ReturnsAsync((Label?)null);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateColorCodeAsync(id, dto));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateColor_ShouldThrowNotFound_WhenLabelDeleted()
        {
            var id = Guid.NewGuid();
            var label = CreateLabel(id);

            var dto = new UpdateColorCodeRequest { ColorCode = "#111" };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>?>()))
                .ReturnsAsync(label);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateColorCodeAsync(id, dto));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateColor_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var label = CreateLabel(id);
            var dto = new UpdateColorCodeRequest { ColorCode = "#222" };

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>?>()))
                .ReturnsAsync(label);

            LabelRepositoryMock
                .Setup(r => r.UpdateAsync(label))
                .Returns(Task.CompletedTask);

            await Sut.UpdateColorCodeAsync(id, dto);

            Assert.Equal("#222", label.ColorCode);

            LabelRepositoryMock.VerifyAll();
        }
    }
}
