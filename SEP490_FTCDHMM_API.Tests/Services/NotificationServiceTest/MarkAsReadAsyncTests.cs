using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.NotificationServiceTests
{
    public class MarkAsReadAsyncTests : NotificationServiceTestBase
    {
        [Fact]
        public async Task MarkAsRead_ShouldDoNothing_WhenNotificationNotFound()
        {
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    notificationId,
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>?>()
                ))
                .ReturnsAsync((Notification?)null);

            await Sut.MarkAsReadAsync(userId, notificationId);

            NotificationRepositoryMock.VerifyAll();
        }


        [Fact]
        public async Task MarkAsRead_ShouldDoNothing_WhenAlreadyRead()
        {
            var userId = Guid.NewGuid();
            var n = CreateNotification();
            n.IsRead = true;

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    n.Id,
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>?>()
                ))
                .ReturnsAsync(n);

            await Sut.MarkAsReadAsync(userId, n.Id);

            NotificationRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task MarkAsRead_ShouldUpdateAndNotify_WhenUnread()
        {
            var userId = Guid.NewGuid();
            var n = CreateNotification();
            n.IsRead = false;

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    n.Id,
                    It.IsAny<Func<IQueryable<Notification>, IQueryable<Notification>>?>()
                ))
                .ReturnsAsync(n);

            NotificationRepositoryMock
                .Setup(r => r.UpdateAsync(n))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(x => x.MarkAsReadAsync(userId, n.Id))
                .Returns(Task.CompletedTask);

            await Sut.MarkAsReadAsync(userId, n.Id);

            Assert.True(n.IsRead);

            NotificationRepositoryMock.VerifyAll();
            NotifierMock.VerifyAll();
        }

    }
}
