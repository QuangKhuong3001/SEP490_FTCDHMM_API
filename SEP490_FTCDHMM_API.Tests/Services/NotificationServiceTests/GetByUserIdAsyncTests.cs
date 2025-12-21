using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.NotificationServiceTests
{
    public class GetNotificationsByUserIdAsyncTests : NotificationServiceTestBase
    {
        [Fact]
        public async Task ShouldReturnGroupedNotifications()
        {
            var userId = Guid.NewGuid();
            var sender = new AppUser { Id = Guid.NewGuid() };
            var now = DateTime.UtcNow;

            var notifications = new List<Notification>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = userId,
                    Sender = sender,
                    Type = NotificationType.Comment,
                    TargetId = Guid.NewGuid(),
                    CreatedAtUtc = now
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = userId,
                    Sender = sender,
                    Type = NotificationType.Comment,
                    TargetId = Guid.NewGuid(),
                    CreatedAtUtc = now
                }
            };

            NotificationRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    1,
                    10,
                    It.IsAny<Expression<Func<Notification, bool>>>(),
                    It.IsAny<Func<IQueryable<Notification>, IOrderedQueryable<Notification>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>>()))
                .ReturnsAsync((notifications, notifications.Count));

            MapperMock
                .Setup(m => m.Map<NotificationResponse>(It.IsAny<Notification>()))
                .Returns((Notification n) => new NotificationResponse
                {
                    Id = n.Id,
                    Type = n.Type,
                    CreatedAtUtc = n.CreatedAtUtc
                });

            MapperMock
                .Setup(m => m.Map<UserInteractionResponse>(sender))
                .Returns(new UserInteractionResponse { Id = sender.Id });

            var result = await Sut.GetNotificationsByUserIdAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 });

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task ShouldReturnEmpty_WhenNoNotifications()
        {
            NotificationRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    1,
                    10,
                    It.IsAny<Expression<Func<Notification, bool>>>(),
                    It.IsAny<Func<IQueryable<Notification>, IOrderedQueryable<Notification>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>>()))
                .ReturnsAsync((Array.Empty<Notification>(), 0));

            var result = await Sut.GetNotificationsByUserIdAsync(
                Guid.NewGuid(),
                new PaginationParams { PageNumber = 1, PageSize = 10 });

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
