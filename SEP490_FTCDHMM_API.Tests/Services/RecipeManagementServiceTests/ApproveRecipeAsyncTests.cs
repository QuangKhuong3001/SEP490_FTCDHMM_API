using FluentAssertions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagement
{
    public class ApproveRecipeAsyncTests : RecipeManagementServiceTestsBase
    {
        [Fact]
        public async Task ApproveRecipeAsync_ShouldThrow_WhenNotFound()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Recipe?)null);

            var act = async () => await Service.ApproveRecipeAsync(Guid.NewGuid(), Guid.NewGuid());

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Công thức không tồn tại");
        }

        [Fact]
        public async Task ApproveRecipeAsync_ShouldThrow_WhenNotPending()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(new Recipe { Status = RecipeStatus.Posted });

            var act = async () => await Service.ApproveRecipeAsync(Guid.NewGuid(), Guid.NewGuid());

            await act.Should().ThrowAsync<AppException>();
        }

        [Fact]
        public async Task ApproveRecipeAsync_ShouldApprove_WhenValid()
        {
            var recipe = new Recipe
            {
                Status = RecipeStatus.Pending,
                AuthorId = Guid.NewGuid()
            };

            RecipeRepoMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
            ))
            .ReturnsAsync(recipe);

            await Service.ApproveRecipeAsync(Guid.NewGuid(), Guid.NewGuid());

            recipe.Status.Should().Be(RecipeStatus.Posted);
            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);
        }
    }
}
