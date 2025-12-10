using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class DeleteLabelAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task DeleteLabel_ShouldThrowNotFound_WhenNotExists()
        {
            var id = Guid.NewGuid();

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>>()))
                .ReturnsAsync((Label?)null);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteLabelAsync(id));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteLabel_ShouldSoftDelete_WhenLabelHasRecipes()
        {
            var id = Guid.NewGuid();
            var label = CreateLabel(id);
            label.Recipes.Add(new Recipe { Id = Guid.NewGuid() });

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>>()))
                .ReturnsAsync(label);

            LabelRepositoryMock
                .Setup(r => r.UpdateAsync(label))
                .Returns(Task.CompletedTask);

            await Sut.DeleteLabelAsync(id);

            Assert.True(label.IsDeleted);

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteLabel_ShouldHardDelete_WhenNoRecipes()
        {
            var id = Guid.NewGuid();
            var label = CreateLabel(id);

            LabelRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>>()))
                .ReturnsAsync(label);

            LabelRepositoryMock
                .Setup(r => r.DeleteAsync(label))
                .Returns(Task.CompletedTask);

            await Sut.DeleteLabelAsync(id);

            LabelRepositoryMock.VerifyAll();
        }
    }
}
