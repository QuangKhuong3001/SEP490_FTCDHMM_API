using FluentAssertions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagement
{
    public class DeleteRecipeAsyncTests : RecipeManagementServiceTestsBase
    {
        [Fact]
        public async Task DeleteRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Recipe?)null);

            var act = async () => await Service.DeleteRecipeByManageAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "test" });

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Công thức không tồn tại");
        }

        [Fact]
        public async Task DeleteRecipeAsync_ShouldThrow_WhenAlreadyDeleted()
        {
            var recipe = new Recipe { Status = RecipeStatus.Deleted };

            RecipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync(recipe);

            var act = async () => await Service.DeleteRecipeByManageAsync(Guid.NewGuid(), Guid.NewGuid(),
                new RecipeManagementReasonRequest { Reason = "test" });

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Công thức không tồn tại");
        }

        [Fact]
        public async Task DeleteRecipeAsync_ShouldPerformDelete_WhenValid()
        {
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var moderatorId = Guid.NewGuid();

            var recipe = new Recipe
            {
                Id = recipeId,
                Status = RecipeStatus.Posted,
                AuthorId = authorId,
                Name = "Recipe1",
                Author = new AppUser
                {
                    Id = authorId,
                    Email = "a@mail.com",
                    FirstName = "A",
                    LastName = "B"
                }
            };

            RecipeRepoMock
                .Setup(r => r.GetByIdAsync(recipeId,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()))
                .ReturnsAsync(recipe);

            TemplateServiceMock
                .Setup(t => t.RenderTemplateAsync(
                    EmailTemplateType.DeleteRecipe,
                    It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("<html></html>");

            MailServiceMock
                .Setup(m => m.SendEmailAsync(
                    "a@mail.com",
                    "<html></html>",
                    "Công thức của bạn đã bị xóa – FitFood Tracker"))
                .Returns(Task.CompletedTask);

            RecipeRepoMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            var req = new RecipeManagementReasonRequest { Reason = "abc" };

            await Service.DeleteRecipeByManageAsync(moderatorId, recipeId, req);

            recipe.Status.Should().Be(RecipeStatus.Deleted);
            recipe.Reason.Should().Be("abc");

            RecipeRepoMock.Verify(r => r.UpdateAsync(recipe), Times.Once);

            MailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    "a@mail.com",
                    "<html></html>",
                    "Công thức của bạn đã bị xóa – FitFood Tracker"),
                Times.Once);
        }

    }
}
