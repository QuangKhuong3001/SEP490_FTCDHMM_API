using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeManagementService : IRecipeManagementService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public RecipeManagementService(
            IRecipeRepository recipeRepository,
            IMailService mailService,
            IEmailTemplateService emailTemplateService)
        {
            _recipeRepository = recipeRepository;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task LockRecipeAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId,
                include: i => i.Include(r => r.Author));

            if (recipe == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            if (recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể khóa công thức này");

            recipe.Status = RecipeStatus.Locked;
            recipe.Reason = request.Reason;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            var author = recipe.Author;

            var placeholders = new Dictionary<string, string>
                    {
                        { "Reason", request.Reason },
                        { "UserName", $"{author!.FirstName} {author.LastName}" },
                        { "RecipeName", recipe.Name },
                    };


            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.LockRecipe, placeholders);

            await _mailService.SendEmailAsync(author!.Email!, htmlBody, "Công thức của bạn đã bị khóa – FitFood Tracker");

            await _recipeRepository.UpdateAsync(recipe);
        }

        public async Task DeleteRecipeByManageAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId,
                            include: i => i.Include(r => r.Author));

            if ((recipe == null) || (recipe.Status == RecipeStatus.Deleted))
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            recipe.Status = RecipeStatus.Deleted;
            recipe.Reason = request.Reason;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            var author = recipe.Author;

            var placeholders = new Dictionary<string, string>
                    {
                        { "Reason", request.Reason },
                        { "UserName", $"{author !.FirstName} {author.LastName}" },
                        { "RecipeName", recipe.Name },
                    };


            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.DeleteRecipe, placeholders);

            await _mailService.SendEmailAsync(author!.Email!, htmlBody, "Công thức của bạn đã bị xóa – FitFood Tracker");

            await _recipeRepository.UpdateAsync(recipe);
        }

        public async Task ApproveRecipeAsync(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId,
                            include: i => i.Include(r => r.Author));

            if (recipe == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            if (recipe.Status != RecipeStatus.Pending)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            recipe.Status = RecipeStatus.Posted;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            var author = recipe.Author;

            await _recipeRepository.UpdateAsync(recipe);
        }

        public async Task RejectRecipeAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId,
                            include: i => i.Include(r => r.Author));

            if (recipe == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            if (recipe.Status != RecipeStatus.Pending)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            recipe.Status = RecipeStatus.Locked;
            recipe.Reason = request.Reason;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            var author = recipe.Author;

            var placeholders = new Dictionary<string, string>
                    {
                        { "Reason", request.Reason },
                        { "UserName", $"{author !.FirstName} {author.LastName}" },
                        { "RecipeName", recipe.Name },
                    };


            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.RejectRecipe, placeholders);

            await _mailService.SendEmailAsync(author!.Email!, htmlBody, "Công thức của bạn đã bị từ chối – FitFood Tracker");

            await _recipeRepository.UpdateAsync(recipe);
        }
    }
}
