using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.NotificationServiceTests
{
    public class MarkAsReadAsyncTests : NotificationServiceTestBase
    {
        [Fact]
        public async Task MarkAsReadAsync_ShouldThrow_WhenNotificationNotFound()
        {
            var userId = Guid.NewGuid();
            var notiId = Guid.NewGuid();

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(notiId, null))
                .ReturnsAsync((Notification?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.MarkAsReadAsync(userId, notiId));

            NotificationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendMarkNotificationAsReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldThrow_WhenUserIsNotReceiver()
        {
            var userId = Guid.NewGuid();
            var notiId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notiId,
                ReceiverId = Guid.NewGuid(),
                IsRead = false
            };

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(notiId, null))
                .ReturnsAsync(notification);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.MarkAsReadAsync(userId, notiId));

            NotificationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendMarkNotificationAsReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldDoNothing_WhenAlreadyRead()
        {
            var userId = Guid.NewGuid();
            var notiId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notiId,
                ReceiverId = userId,
                IsRead = true
            };

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(notiId, null))
                .ReturnsAsync(notification);

            await Sut.MarkAsReadAsync(userId, notiId);

            NotificationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendMarkNotificationAsReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldUpdateAndNotify_WhenValid()
        {
            var userId = Guid.NewGuid();
            var notiId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notiId,
                ReceiverId = userId,
                IsRead = false
            };

            NotificationRepositoryMock
                .Setup(r => r.GetByIdAsync(notiId, null))
                .ReturnsAsync(notification);

            NotificationRepositoryMock
                .Setup(r => r.UpdateAsync(notification))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(n => n.SendMarkNotificationAsReadAsync(userId, notiId))
                .Returns(Task.CompletedTask);

            await Sut.MarkAsReadAsync(userId, notiId);

            NotificationRepositoryMock.Verify(r => r.UpdateAsync(notification), Times.Once);
            NotifierMock.Verify(n => n.SendMarkNotificationAsReadAsync(userId, notiId), Times.Once);

            Assert.True(notification.IsRead);
        }
    }
}
