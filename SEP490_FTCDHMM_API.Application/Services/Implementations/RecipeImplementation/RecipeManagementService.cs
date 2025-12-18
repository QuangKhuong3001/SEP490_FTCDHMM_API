using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeManagementService : IRecipeManagementService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ICacheService _cacheService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealtimeNotifier _notifier;
        private readonly IS3ImageService _s3ImageService;

        public RecipeManagementService(
            IRecipeRepository recipeRepository,
            IMailService mailService,
            ICacheService cacheService,
            INotificationRepository notificationRepository,
            IRealtimeNotifier notifier,
            IEmailTemplateService emailTemplateService,
            IS3ImageService s3ImageService)
        {
            _recipeRepository = recipeRepository;
            _cacheService = cacheService;
            _mailService = mailService;
            _notificationRepository = notificationRepository;
            _notifier = notifier;
            _emailTemplateService = emailTemplateService;
            _s3ImageService = s3ImageService;
        }

        private async Task CreateAndSendNotificationAsync(Guid? senderId, Guid receiverId, NotificationType type, Guid targetId)
        {
            if (senderId == receiverId)
                return;

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Type = type,
                TargetId = targetId,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _notificationRepository.AddAsync(notification);

            string? recipeImageUrl = null;
            var recipe = await _recipeRepository.GetByIdAsync(targetId,
                include: r => r.Include(rec => rec.Image));
            if (recipe?.Image != null)
            {
                recipeImageUrl = _s3ImageService.GeneratePreSignedUrl(recipe.Image.Key);
            }

            var notificationResponse = new
            {
                Id = notification.Id,
                Type = type,
                TargetId = targetId,
                IsRead = false,
                CreatedAtUtc = notification.CreatedAtUtc,
                Senders = Array.Empty<object>(),
                RecipeImageUrl = recipeImageUrl
            };

            await _notifier.SendNotificationAsync(receiverId, notificationResponse);
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
            await this.CreateAndSendNotificationAsync(null, recipe.AuthorId, NotificationType.LockRecipe, recipe.Id);

            await _recipeRepository.UpdateAsync(recipe);
            await _cacheService.RemoveByPrefixAsync("recipe");
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
            await this.CreateAndSendNotificationAsync(null, recipe.AuthorId, NotificationType.DeleteRecipe, recipe.Id);

            await _recipeRepository.UpdateAsync(recipe);
            await _cacheService.RemoveByPrefixAsync("recipe");
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

            await this.CreateAndSendNotificationAsync(null, recipe.AuthorId, NotificationType.ApproveRecipe, recipe.Id);

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
            await this.CreateAndSendNotificationAsync(null, recipe.AuthorId, NotificationType.RejectRecipe, recipe.Id);

            await _recipeRepository.UpdateAsync(recipe);
        }
    }
}
