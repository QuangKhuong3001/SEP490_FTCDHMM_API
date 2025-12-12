using FluentAssertions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagementServiceTests
{
    public class RejectRecipeAsyncTests : RecipeManagementServiceTestsBase
    {
        [Fact]
        public async Task RejectRecipeAsync_ShouldThrow_WhenNotFound()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Recipe?)null);

            var act = async () => await Service.RejectRecipeAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "abc" });

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Công thức không tồn tại");
        }

        [Fact]
        public async Task RejectRecipeAsync_ShouldThrow_WhenNotPending()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(new Recipe { Status = RecipeStatus.Posted });

            var act = async () => await Service.RejectRecipeAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "abc" });

            await act.Should().ThrowAsync<AppException>();
        }

        [Fact]
        public async Task RejectRecipeAsync_ShouldLock_WhenValid()
        {
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var moderatorId = Guid.NewGuid();

            var recipe = new Recipe
            {
                Id = recipeId,
                Status = RecipeStatus.Pending,
                AuthorId = authorId,
                Name = "Recipe A",
                Author = new AppUser
                {
                    Id = authorId,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@mail.com"
                }
            };

            RecipeRepoMock.Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()))
                .ReturnsAsync(recipe);

            TemplateServiceMock.Setup(t => t.RenderTemplateAsync(
                EmailTemplateType.RejectRecipe,
                It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("<html></html>");

            var req = new RecipeManagementReasonRequest { Reason = "bad" };

            await Service.RejectRecipeAsync(moderatorId, recipeId, req);

            recipe.Status.Should().Be(RecipeStatus.Locked);
            recipe.Reason.Should().Be("bad");

            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);

            MailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    recipe.Author.Email,
                    "<html></html>",
                    "Công thức của bạn đã bị từ chối – FitFood Tracker"
                ),
                Times.Once);
        }

    }
}
