using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class UnLockUserAccountAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task UnLockUserAccount_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.UnLockUserAccountAsync(userId));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task UnLockUserAccount_ShouldThrow_WhenUserNotLocked()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(-1);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.UnLockUserAccountAsync(userId));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task UnLockUserAccount_ShouldUnLock_WhenValid()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.LockoutEnd = DateTime.UtcNow.AddHours(2);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(EmailTemplateType.UnlockAccount, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("<html></html>");

            var result = await Sut.UnLockUserAccountAsync(userId);

            Assert.Equal(user.Email, result.Email);
            Assert.Null(user.LockoutEnd);
            Assert.Null(user.LockReason);

            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
            MailServiceMock.Verify(m => m.SendEmailAsync(user.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
