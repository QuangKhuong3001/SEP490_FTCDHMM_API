using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class LockUserAccountAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task LockUserAccount_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var dto = new LockRequest { Day = 3, Reason = "Testing" };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((AppUser)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.LockUserAccountAsync(userId, dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }


        [Fact]
        public async Task LockUserAccount_ShouldThrow_WhenUserAlreadyLocked()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.LockoutEnd = DateTime.UtcNow.AddHours(1);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            var dto = new LockRequest { Day = 3, Reason = "test" };

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.LockUserAccountAsync(userId, dto));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }



        [Fact]
        public async Task LockUserAccount_ShouldLock_WhenValidRequest()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.LockoutEnd = null;

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("<html></html>");

            var dto = new LockRequest { Day = 5, Reason = "Violation" };

            var result = await Sut.LockUserAccountAsync(userId, dto);

            Assert.Equal(user.Email, result.Email);
            Assert.NotNull(result.LockoutEnd);
            Assert.True(result.LockoutEnd > DateTime.UtcNow);

            UserRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AppUser>()), Times.Once);
            MailServiceMock.Verify(m => m.SendEmailAsync(user.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
