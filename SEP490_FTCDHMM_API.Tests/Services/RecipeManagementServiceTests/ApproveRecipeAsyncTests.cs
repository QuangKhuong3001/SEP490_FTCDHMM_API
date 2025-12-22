using FluentAssertions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagementServiceTests
{
    public class ApproveRecipeAsyncTests : RecipeManagementServiceTestsBase
    {
        [Fact]
        public async Task ApproveRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Service.ApproveRecipeAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task ApproveRecipeAsync_ShouldThrow_WhenStatusNotPending()
        {
            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(new Recipe { Status = RecipeStatus.Posted });

            await Assert.ThrowsAsync<AppException>(() =>
                Service.ApproveRecipeAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task ApproveRecipeAsync_ShouldApprove_WhenValid()
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Status = RecipeStatus.Pending,
                AuthorId = Guid.NewGuid()
            };

            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()
                ))
                .ReturnsAsync(recipe);


            await Service.ApproveRecipeAsync(Guid.NewGuid(), recipe.Id);

            recipe.Status.Should().Be(RecipeStatus.Posted);
            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);
        }
    }

}