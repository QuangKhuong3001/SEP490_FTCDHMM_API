using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.NotificationServiceTests
{
    public abstract class NotificationServiceTestBase
    {
        protected Mock<INotificationRepository> NotificationRepositoryMock { get; }
        protected Mock<IRealtimeNotifier> NotifierMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected NotificationService Sut { get; }

        protected NotificationServiceTestBase()
        {
            NotificationRepositoryMock = new Mock<INotificationRepository>(MockBehavior.Strict);
            NotifierMock = new Mock<IRealtimeNotifier>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);

            Sut = new NotificationService(
                NotificationRepositoryMock.Object,
                MapperMock.Object,
                NotifierMock.Object
            );
        }

        protected Notification CreateNotification(Guid? id = null)
        {
            return new Notification
            {
                Id = id ?? Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid(),
                TargetId = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                Type = NotificationType.Comment
            };
        }
    }
}
