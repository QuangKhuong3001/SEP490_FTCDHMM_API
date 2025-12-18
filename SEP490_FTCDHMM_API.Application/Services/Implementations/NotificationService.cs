using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _notifier;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IS3ImageService _s3ImageService;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IRealtimeNotifier notifier, IRecipeRepository recipeRepository, IS3ImageService s3ImageService)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _notifier = notifier;
            _recipeRepository = recipeRepository;
            _s3ImageService = s3ImageService;
        }

        public async Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetAllAsync(
                n => n.ReceiverId == userId,
                q => q
                    .Include(x => x.Sender)
                        .ThenInclude(s => s!.Avatar)
                    .OrderByDescending(x => x.CreatedAtUtc)
            );

            var grouped = notifications
                .GroupBy(n => new
                {
                    n.TargetId,
                    n.Type,
                    Date = n.CreatedAtUtc.Date
                })
                .Select(g =>
                {
                    var response = _mapper.Map<NotificationResponse>(g.First());

                    if (response.Type == NotificationType.System)
                    {
                        response.Senders = new List<UserInteractionResponse>();
                        if (response.TargetId.HasValue)
                        {
                            var recipe = _recipeRepository.GetByIdAsync(response.TargetId.Value,
                                include: r => r.Include(rec => rec.Image)).Result;
                            if (recipe?.Image != null)
                            {
                                response.RecipeImageUrl = _s3ImageService.GeneratePreSignedUrl(recipe.Image.Key);
                            }
                        }
                    }
                    else
                    {
                        response.Senders = g
                            .Where(n => n.Sender != null)
                            .Select(n => _mapper.Map<UserInteractionResponse>(n.Sender))
                            .DistinctBy(s => s.Id)
                            .ToList();
                    }

                    return response;
                })
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToList();

            return grouped;
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Không tìm thấy thông báo.");

            if (notification.ReceiverId != userId)
            {
                throw new AppException(AppResponseCode.FORBIDDEN, "Bạn không có quyền thực hiện hành động này.");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
                await _notifier.SendMarkNotificationAsReadAsync(userId, notificationId);
            }
        }
    }
}
