using FluentAssertions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagementServiceTests
{
    public class LockRecipeAsyncTests : RecipeManagementServiceTestsBase
    {
        [Fact]
        public async Task LockRecipeAsync_ShouldThrow_WhenNotFound()
        {
            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Service.LockRecipeAsync(Guid.NewGuid(), Guid.NewGuid(), new()));
        }

        [Fact]
        public async Task LockRecipeAsync_ShouldThrow_WhenStatusNotPosted()
        {
            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(new Recipe { Status = RecipeStatus.Pending });

            await Assert.ThrowsAsync<AppException>(() =>
                Service.LockRecipeAsync(Guid.NewGuid(), Guid.NewGuid(), new()));
        }

        [Fact]
        public async Task LockRecipeAsync_ShouldLock_WhenValid()
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Status = RecipeStatus.Posted,
                AuthorId = Guid.NewGuid(),
                Author = new AppUser { Email = "a@a.com", FirstName = "A", LastName = "B" }
            };

            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            TemplateServiceMock
                .Setup(t => t.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            MailServiceMock
                .Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await Service.LockRecipeAsync(Guid.NewGuid(), recipe.Id, new RecipeManagementReasonRequest { Reason = "spam" });

            recipe.Status.Should().Be(RecipeStatus.Locked);
            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);
            CacheServiceMock.Verify(c => c.RemoveByPrefixAsync("recipe"), Times.Once);
        }
    }

}
