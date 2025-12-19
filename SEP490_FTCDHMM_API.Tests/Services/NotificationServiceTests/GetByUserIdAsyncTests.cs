using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.NotificationServiceTests
{
    public class GetByUserIdAsyncTests : NotificationServiceTestBase
    {
        [Fact]
        public async Task GetByUserIdAsync_ShouldGroupNotifications_ByTargetTypeAndDate()
        {
            var userId = Guid.NewGuid();

            var n1 = CreateNotification();
            var n2 = CreateNotification();
            n2.TargetId = n1.TargetId;
            n2.Type = n1.Type;
            n2.CreatedAtUtc = n1.CreatedAtUtc;

            var notifications = new List<Notification> { n1, n2 };

            NotificationRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Notification, bool>>>(),
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>?>()
                ))
                .ReturnsAsync(notifications);

            var mapped = new NotificationResponse
            {
                Id = n1.Id,
                CreatedAtUtc = n1.CreatedAtUtc,
                Type = n1.Type
            };

            MapperMock
                .Setup(m => m.Map<NotificationResponse>(n1))
                .Returns(mapped);

            var pagination = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sut.GetNotificationsByUserIdAsync(userId, pagination);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);

            NotificationRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEmptySenders_WhenTypeIsSystem()
        {
            var userId = Guid.NewGuid();
            var n = CreateNotification();
            n.Type = NotificationType.System;

            var notifications = new List<Notification> { n };

            NotificationRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Notification, bool>>>(),
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>?>()
                ))
                .ReturnsAsync(notifications);

            var mapped = new NotificationResponse
            {
                Id = n.Id,
                Type = NotificationType.System,
                CreatedAtUtc = n.CreatedAtUtc
            };

            MapperMock
                .Setup(m => m.Map<NotificationResponse>(n))
                .Returns(mapped);

            var pagination = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sut.GetNotificationsByUserIdAsync(userId, pagination);

            Assert.Single(result.Items);
            Assert.Empty(result.Items.First().Senders);

            NotificationRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
