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
        public async Task LockRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Recipe?)null);

            var act = async () => await Service.LockRecipeAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "test" });

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Công thức không tồn tại");
        }

        [Fact]
        public async Task LockRecipeAsync_ShouldThrow_WhenNotPosted()
        {
            var recipe = new Recipe { Status = RecipeStatus.Pending };

            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>())).ReturnsAsync(recipe);

            var act = async () => await Service.LockRecipeAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "test" });

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Không thể khóa công thức này");
        }

        [Fact]
        public async Task LockRecipeAsync_ShouldUpdateAndSendMail_WhenValid()
        {
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var moderatorId = Guid.NewGuid();

            var recipe = new Recipe
            {
                Id = recipeId,
                Status = RecipeStatus.Posted,
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
                EmailTemplateType.LockRecipe,
                It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("<html></html>");

            var req = new RecipeManagementReasonRequest { Reason = "Vi phạm" };

            await Service.LockRecipeAsync(moderatorId, recipeId, req);

            recipe.Status.Should().Be(RecipeStatus.Locked);
            recipe.Reason.Should().Be("Vi phạm");

            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);

            MailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    "test@mail.com",
                    "<html></html>",
                    "Công thức của bạn đã bị khóa – FitFood Tracker"
                ),
                Times.Once);
        }
    }
}
